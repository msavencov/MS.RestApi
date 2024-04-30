using System.Collections.Generic;
using System.Text;
using MS.RestApi.SourceGenerator.Descriptors;
using MS.RestApi.SourceGenerator.Helpers;
using MS.RestApi.SourceGenerator.Helpers.Pipe;

namespace MS.RestApi.SourceGenerator.Generators.Server;

internal class AddServices : IMiddleware<ApiGenContext>
{
    public void Execute(ApiGenContext context)
    {
        foreach (var (serviceName, requests) in context.Services.AsTuple())
        {
            var serviceInterface = context.Options.ServerConventions.ServiceInterface(serviceName);
            var service = new ApiGenSourceCode
            {
                Name = $"{serviceInterface.Namespace}.{serviceInterface.Name}.g.cs",
                Source = BuildServiceSourceCode(context, serviceName, requests),
            };
            context.Result.Add(service);
        }
    }
    
    private string BuildServiceSourceCode(ApiGenContext context, string serviceName, IEnumerable<ApiRequestDescriptor> actions)
    {
        var builder = new StringBuilder();
        var writer = new IndentedWriter(builder, 0);
        
        var symbol = context.Symbols;
        var conventions = context.Options.ServerConventions;
        var service = conventions.ServiceInterface(serviceName);
        
        writer.WriteHeaderLines();
        writer.WriteLine($"namespace {service.Namespace}");
        writer.WriteBlock(nsw =>
        {
            nsw.WriteLine($"/// <inheritdoc/>");
            nsw.WriteLine($"public interface {service.Name} : {symbol.IApiService.ToDisplayString()}");
            nsw.WriteBlock(cw =>
            {
                foreach (var request in actions)
                {
                    var ct = symbol.CancellationToken.ToDisplayString();
                    var model = request.Request.ToDisplayString();
                    
                    cw.WriteLine($"/// <inheritdoc cref=\"{model}\"/>");
                    cw.WriteLine($"{request.ReturnType} {request.Request.Name}({model} model, {ct} token = default);");
                }
            });
        });
            
        return builder.ToString();
    }
}
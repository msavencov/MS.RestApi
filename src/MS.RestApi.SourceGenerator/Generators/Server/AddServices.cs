using System.Collections.Generic;
using System.Text;
using MS.RestApi.SourceGenerator.Descriptors;
using MS.RestApi.SourceGenerator.Extensions;
using MS.RestApi.SourceGenerator.Extensions.Pipe;

namespace MS.RestApi.SourceGenerator.Generators.Server;

internal class AddServices : IMiddleware<ApiGenContext>
{
    public void Execute(ApiGenContext context)
    {
        foreach (var (serviceName, requests) in context.Services)
        {
            var service = new ApiGenSourceCode
            {
                Name = $"{context.Options.ServerConventions.ServiceNamespace}.{serviceName}.cs",
                Source = BuildServiceSourceCode(context, serviceName, requests),
            };
            context.Result.Add(service);
        }
    }
    
    private string BuildServiceSourceCode(ApiGenContext context, string serviceName, IEnumerable<ApiRequestDescriptor> actions)
    {
        var builder = new StringBuilder();
        var writer = new IndentedWriter(builder, 0);
        
        var symbol = context.KnownSymbols;
        var conventions = context.Options.ServerConventions;
        var service = conventions.ServiceInterface(serviceName);
        
        writer.WriteLine($"namespace {service.Namespace}");
        writer.WriteBlock(nsw =>
        {
            nsw.WriteLine($"public interface {service.Name} : {symbol.IApiService.ToDisplayString()}");
            nsw.WriteBlock(cw =>
            {
                foreach (var request in actions)
                {
                    var ct = symbol.CancellationToken.ToDisplayString();
                    var model = request.Request.ToDisplayString();
                    var returnResult = request.GetResponseTypeName(symbol.Task);
                    
                    var method = $"{returnResult} {serviceName}({model} model, {ct} token = default);";

                    cw.WriteLine(method);
                }
            });
        });
            
        return builder.ToString();
    }
}
using System.Collections.Generic;
using System.Text;
using MS.RestApi.SourceGenerator.Descriptors;
using MS.RestApi.SourceGenerator.Exceptions;
using MS.RestApi.SourceGenerator.Helpers;
using MS.RestApi.SourceGenerator.Helpers.Pipe;

namespace MS.RestApi.SourceGenerator.Generators.Server;

internal class AddMediator : IMiddleware<ApiGenContext>
{
    public void Execute(ApiGenContext context)
    {
        var symbols = context.Symbols;
        
        if (symbols.IMediator is null)
        {
            throw ApiGenException.RequiredAssemblyReference("Mediator");
        }
        
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
            nsw.WriteLine($"public interface {service.Name} : {symbol.IApiService.ToDisplayString()}");
            nsw.WriteBlock("", "", cw =>
            {
                foreach (var descriptor in actions)
                {
                    var @interface = descriptor.Response switch
                    {
                        null => symbol.MediatorRequestHandler1.Construct(descriptor.Request),
                        { } response => symbol.MediatorRequestHandler2.Construct(descriptor.Request, response)
                    };
                    cw.WriteLine($", {@interface.ToDisplayString()}");
                }
                cw.WriteLine(";");
            });
        });
            
        return builder.ToString();
    }
}
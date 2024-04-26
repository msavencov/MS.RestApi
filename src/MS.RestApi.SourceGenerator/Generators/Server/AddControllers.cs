using System.Text;
using MS.RestApi.SourceGenerator.Exceptions;
using MS.RestApi.SourceGenerator.Extensions;
using MS.RestApi.SourceGenerator.Extensions.Pipe;

namespace MS.RestApi.SourceGenerator.Generators.Server;

internal class AddControllers : IMiddleware<ApiGenContext>
{
    public void Execute(ApiGenContext context)
    {
        var symbols = context.Symbols;
        var options = context.Options;
        var conventions = options.ServerConventions;
        
        if (options.UseMediatorHandlers && symbols.IMediator is null)
        {
            throw ApiGenException.RequiredAssemblyReference("Mediator");
        }

        foreach (var (serviceName, requests) in context.Services)
        {
            var builder = new StringBuilder();
            var writer = new IndentedWriter(builder, 0);

            var service = conventions.ServiceInterface(serviceName);
            var serviceFullname = options.UseMediatorHandlers ? symbols.IMediator?.ToDisplayString() : $"{service.Namespace}.{service.Name}";
            var controller = conventions.ControllerName(serviceName);
            var methodAttribute = symbols.HttpPostAttribute.ToDisplayString();
            var routeAttribute = symbols.RouteAttribute.ToDisplayString();
            var fromAttribute = symbols.FromBodyAttribute.ToDisplayString();
            
            writer.WriteHeaderLines();
            writer.WriteLine($"namespace {controller.Namespace}");
            writer.WriteBlock(ns =>
            {
                ns.WriteLine($"[{symbols.ApiControllerAttribute.ToDisplayString()}]");
                ns.WriteLine($"public class {controller.Name}({serviceFullname} service) : {symbols.ControllerBase.ToDisplayString()}");
                ns.WriteBlock(cb =>
                {
                    foreach (var item in requests)
                    {
                        var request = item.Request;
                        var serviceMethodName = options.UseMediatorHandlers ? "Send" : request.Name;
                        var requestType = request.ToDisplayString();
                        
                        cb.WriteLine($"/// <inheritdoc cref=\"{requestType}\"/>");
                        cb.WriteLine($"[{methodAttribute}, {routeAttribute}(\"{options.GetRoute(item.Endpoint)}\")]");
                        cb.WriteLine($"public {item.ReturnType} {request.Name}([{fromAttribute}] {requestType} model, {symbols.CancellationToken.ToDisplayString()} token)");
                        cb.WriteBlock(mb =>
                        {
                            mb.WriteLine($"return service.{serviceMethodName}(model, token);");
                        });
                    }
                });
            });
            
            context.Result.Add(new ApiGenSourceCode
            {
                Name = $"{controller.Namespace}.{controller.Name}.cs",
                Source = builder.ToString()
            });
        }
    }
}
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using MS.RestApi.SourceGenerator.Exceptions;
using MS.RestApi.SourceGenerator.Helpers;
using MS.RestApi.SourceGenerator.Helpers.Pipe;

namespace MS.RestApi.SourceGenerator.Generators.Server;

internal class AddControllers : IMiddleware<ApiGenContext>
{
    public void Execute(ApiGenContext context)
    {
        var symbols = context.Symbols;
        var options = context.Options;
        var conventions = options.ServerConventions;
        var useMediator = options.GenerateControllers == GenerateControllers.WithMediator;
        var comparer = SymbolEqualityComparer.Default;
        
        if (useMediator && symbols.IMediator is null)
        {
            throw ApiGenException.RequiredAssemblyReference("Mediator");
        }

        foreach (var (serviceName, requests) in context.Services.AsTuple())
        {
            var builder = new StringBuilder();
            var writer = new IndentedWriter(builder, 0);

            var service = conventions.ServiceInterface(serviceName);
            var serviceFullname = useMediator ? symbols.IMediator?.ToDisplayString() : $"{service.Namespace}.{service.Name}";
            var controller = conventions.ControllerName(serviceName);
            var methodAttribute = symbols.HttpPostAttribute.ToDisplayString();
            var fromRouteAttribute = symbols.FromRouteAttribute;
            var fromFormAttribute = symbols.FromFormAttribute;
            var fromBodyAttribute = symbols.FromBodyAttribute;
            
            writer.WriteHeaderLines();
            writer.WriteLine($"namespace {controller.Namespace}");
            writer.WriteBlock(ns =>
            {
                ns.WriteLine($"[{symbols.ApiControllerAttribute.ToDisplayString()}]");
                ns.WriteLine($"public class {controller.Name}({serviceFullname} service) : {symbols.ControllerBase.ToDisplayString()}");
                ns.WriteBlock(cb =>
                {
                    foreach (var action in requests)
                    {
                        var request = action.Request;
                        var model = "model";
                        var requestType = request.ToDisplayString();
                        var serviceMethodName = useMediator ? "Send" : request.Name;
                        var routeArguments = action.GetRouteParameters();
                        var routeArgumentsList = routeArguments.Select(t => $"[{fromRouteAttribute.ToDisplayString()}] {t.Type.ToDisplayString()} {t.Name}, ").Join();
                        var fromAttribute = default(string);
                        var actionMethodName = $"Post{request.Name}";
                        var binderAttribute = symbols.BindFormFileAttribute.ToDisplayString();
                        
                        cb.WriteLine($"/// <inheritdoc cref={requestType.Quote()}/>");
                        cb.WriteLine($"[{methodAttribute}({options.GetRoute(action.Endpoint).Quote()})]");

                        foreach (var property in action.GetAttachmentParameters())
                        {
                            cb.WriteLine($"[{binderAttribute}({model.Quote()}, {property.Name.Quote()})]");
                            fromAttribute ??= fromFormAttribute.ToDisplayString();
                        }

                        fromAttribute ??= fromBodyAttribute.ToDisplayString();
                        
                        cb.WriteLine($"public {action.ReturnType} {actionMethodName}({routeArgumentsList}[{fromAttribute}, ] {requestType} {model}, {symbols.CancellationToken.ToDisplayString()} token)");
                        cb.WriteBlock(mb =>
                        {
                            if (request.IsRecord)
                            {
                                foreach (var routeArgument in routeArguments)
                                {
                                    mb.WriteLine($"{model} = {model} with {{ {routeArgument.Name} = {routeArgument.Name} }};");
                                }
                            }
                            else
                            {
                                foreach (var routeArgument in routeArguments)
                                {
                                    mb.WriteLine($"{model}.{routeArgument.Name} = {routeArgument.Name};");
                                }
                            }
                            
                            mb.WriteLine($"return service.{serviceMethodName}({model}, token);");
                        });
                    }
                });
            });
            
            context.Result.Add(new ApiGenSourceCode
            {
                Name = $"{controller.Namespace}.{controller.Name}.g.cs",
                Source = builder.ToString()
            });
        }
    }
}
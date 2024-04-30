using System.Text;
using MS.RestApi.SourceGenerator.Helpers;
using MS.RestApi.SourceGenerator.Helpers.Pipe;

namespace MS.RestApi.SourceGenerator.Generators.Client;

internal class AddClientImplementation : IMiddleware<ApiGenContext>
{
    public void Execute(ApiGenContext context)
    {
        var config = context.Options;
        var symbol = context.Symbols;
        var conventions = config.ClientConventions;

        var requestHandler = conventions.GetRequestHandler();
        var requestHandlerInterface = $"{requestHandler.Namespace}.{requestHandler.Name}";
        
        foreach (var (service, actions) in context.Services.AsTuple())
        {
            var builder = new StringBuilder();
            var writer = new IndentedWriter(builder, 0);
            var api = conventions.GetApiService(service);
            var client = conventions.GetClientService(service);
            
            writer.WriteHeaderLines();
            writer.WriteLine($"namespace {client.Namespace}");
            writer.WriteBlock(nsw =>
            {
                nsw.WriteLine($"internal class {client.Name}({requestHandlerInterface} handler) : {api.Namespace}.{api.Name}");
                nsw.WriteBlock(cw =>
                {
                    foreach (var action in actions)
                    {
                        var request = action.Request;
                        var requestType = request.ToDisplayString();
                        var cancellationToken = symbol.CancellationToken.ToDisplayString();
                        
                        cw.WriteLine($"public {action.ReturnType} {request.Name}({requestType} model, {cancellationToken} token = default)");
                        cw.WriteBlock(mw =>
                        {
                            var resource = action.Endpoint.Quote();
                            
                            if (action.GetRouteArguments() is { Count: > 0 } routeArguments)
                            {
                                mw.WriteLine($"var resource = {action.Endpoint.Quote()};");
                            
                                foreach (var routeArgument in routeArguments)
                                {
                                    // resource = resource.Replace("{Id}", model.Id);
                                    var argument = routeArgument.Name.Quote(QuoteType.Braces);
                                    var propertyName = $"model.{routeArgument.Name}";
                                    var value = $"{propertyName} is {{}} " +
                                                $"? {propertyName}.ToString(System.Globalization.CultureInfo.InvariantCulture) " +
                                                $": throw new System.ArgumentNullException(nameof({propertyName}))";

                                    mw.WriteLine($"resource = resource.Replace({argument.Quote()}, {value});");
                                }

                                resource = "resource";
                            }
                            var handlerTypeArgs = requestType;

                            if (action.Response is { } response)
                            {
                                handlerTypeArgs += $", {response.ToDisplayString()}";
                            }
                            
                            mw.WriteLine($"return handler.HandleAsync<{handlerTypeArgs}>({resource}, model, token);");
                        });
                    }
                });
            });
                
            var sourceCode = new ApiGenSourceCode
            {
                Name = $"{conventions.ServicesImplNamespace}.{service}.g.cs",
                Source = builder.ToString()
            };
                
            context.Result.Add(sourceCode);
        }
    }
}
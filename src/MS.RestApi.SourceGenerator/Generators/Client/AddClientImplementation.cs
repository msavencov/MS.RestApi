using System.Text;
using MS.RestApi.SourceGenerator.Descriptors;
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
                        BuildClientMethod(cw, action, symbol);
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

    private void BuildClientMethod(IndentedWriter cw, ApiRequestDescriptor action, KnownSymbols symbol)
    {
        var request = action.Request;
        var requestType = request.ToDisplayString();
        var cancellationToken = symbol.CancellationToken.ToDisplayString();
        var dictionary = "System.Collections.Generic.Dictionary";
        var factory = symbol.RequestFactory.ToDisplayString();
        var formRequestBuilder = symbol.FormRequestBuilder.ToDisplayString();
        var jsonBodyRequestBuilder = symbol.JsonBodyRequestBuilder.ToDisplayString();
        
        cw.WriteLine($"public {action.ReturnType} {request.Name}({requestType} model, {cancellationToken} token = default)");
        cw.WriteBlock(mw =>
        {
            var resource = action.Endpoint;
            var method = "post";
            var builder = "builder";
            
            mw.WriteLine($"var parameters = new {dictionary}<string, object>();");

            foreach (var routeParameter in action.GetRouteParameters())
            {
                mw.WriteLine($"parameters.Add({routeParameter.Name}, model.{routeParameter.Name});");
            }
            
            mw.WriteLine($"var factory = new {factory}({method.Quote()}, {resource.Quote()}, parameters);");

            if (action.GetAttachmentParameters() is { Count: > 0 } attachments)
            {
                mw.WriteLine($"var attachments = new {dictionary}<string, object>();");
                foreach (var attachment in attachments)
                {
                    mw.WriteLine($"attachments.Add({attachment.Name.Quote()}, model.{attachment.Name});");
                }
                mw.WriteLine($"var {builder} = new {formRequestBuilder}(factory, model, attachments);");
            }
            else
            {
                mw.WriteLine($"var {builder} = new {jsonBodyRequestBuilder}(factory, model);");
            }
            
            var handlerTypeArgs = requestType;

            if (action.Response is { } response)
            {
                handlerTypeArgs += $", {response.ToDisplayString()}";
            }

            mw.WriteLine($"return handler.HandleAsync<{handlerTypeArgs}>({resource.Quote()}, model, token);");
        });
    }
}
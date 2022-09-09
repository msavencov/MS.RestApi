using MS.RestApi.SourceGenerator.Builder;
using MS.RestApi.SourceGenerator.Extensions;
using MS.RestApi.SourceGenerator.Pipe;
using MS.RestApi.SourceGenerator.Server.Helpers;
using MS.RestApi.SourceGenerator.Utils;

namespace MS.RestApi.SourceGenerator.Server
{
    internal class AddControllersWithMediator : IMiddleware<ApiGenContext>
    {
        public void Execute(ApiGenContext context)
        {
            var services = context.Requests.AsServices();

            foreach (var service in services)
            {
                context.SourceCode.Add(new ApiGenSourceCode
                {
                    Name = $"{context.Config.ServerControllerNamespace}.{service.ServiceName}Controller.cs",
                    Source = BuildControllerSource(context, service),
                });
            }
        }

        private string BuildControllerSource(ApiGenContext context, ApiGenService service)
        {
            var writer = new IndentedWriter();
            var symbols = context.KnownSymbols;

            var controllerName = $"{service.ServiceName}Controller"; 
            var apiControllerAttributeName = symbols.ApiControllerAttribute.FullName();
            
            writer.WriteLine($"namespace {context.Config.ServerControllerNamespace}");
            writer.WriteBlock(nsw =>
            {
                nsw.WriteLine($"[{apiControllerAttributeName}]");
                nsw.WriteLine($"public class {controllerName} : {symbols.ControllerBase.FullName()}");
                nsw.WriteBlock(cw =>
                {
                    cw.WriteLine($"private readonly {symbols.Mediator.FullName()} _mediator;");
                    cw.WriteLine();

                    cw.WriteLine($"public {controllerName}({symbols.Mediator.FullName()} mediator)");
                    cw.WriteBlock(mw =>
                    {
                        mw.WriteLine("_mediator = mediator;");
                    });
                    cw.WriteLine();
                    
                    foreach (var operation in service.Operations)
                    {
                        var action = operation.AsAction(context);

                        cw.WriteLine($"/// <inheritdoc cref=\"{action.ModelTypeName}\"/>");
                        cw.WriteLine($"[{action.HttpMethodAttribute}, {action.HttpRouteAttribute}]");
                        cw.WriteLine($"public async {action.ResponseTypeName} {action.ActionName}([{action.ModelFromAttributeName}] {action.ModelTypeName} model, {symbols.CancellationToken.FullName()} ct)");
                        cw.WriteBlock(mb =>
                        {
                            mb.WriteLine($"{(action.ResponseTypeIsVoid ? "" : "return")} await _mediator.Send(model, ct);");
                        });
                    }
                });
            });
            
            return writer.ToString();
        }
    }
}
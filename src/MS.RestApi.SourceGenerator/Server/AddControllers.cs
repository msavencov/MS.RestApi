using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using MS.RestApi.Abstractions;
using MS.RestApi.SourceGenerator.Builder;
using MS.RestApi.SourceGenerator.Common;
using MS.RestApi.SourceGenerator.Extensions;
using MS.RestApi.SourceGenerator.Pipe;
using MS.RestApi.SourceGenerator.Server.Helpers;
using MS.RestApi.SourceGenerator.Utils;

namespace MS.RestApi.SourceGenerator.Server
{
    internal class AddControllers : IMiddleware<ApiGenContext>
    {
        public void Execute(ApiGenContext context)
        {
            if (context.Requests.Count == 0)
            {
                throw new ApiGenException(10, "No any request found.");
            }

            foreach (var service in context.Requests.AsServices())
            {
                var controller = new ApiGenSourceCode
                {
                    Name = $"{context.Config.ServerControllerNamespace}.{service.ServiceName}Controller.cs",
                    Source = BuildControllerSourceCode(context, service.ServiceName, service.Operations),
                };
                context.SourceCode.Add(controller);
            }
        }

        private string BuildControllerSourceCode(ApiGenContext context, string serviceName, IEnumerable<ApiGenRequest> requests)
        {
            var builder = new StringBuilder();
            var writer = new IndentedWriter(builder, 0);
            var symbols = context.KnownSymbols;
            
            var controllerClassName = $"{serviceName}Controller";
            var apiControllerName = symbols.ApiControllerAttribute.FullName();
            var routeAttributeName = symbols.RouteAttribute.FullName();
            var serviceFullname = $"{context.Config.ServerServiceNamespace}.{ApiGenSymbols.GetServiceInterfaceName(serviceName)}";
            
            writer.WriteLine($"namespace {context.Config.ServerControllerNamespace}");
            writer.WriteBlock(nsw =>
            {
                nsw.WriteLine($"[{apiControllerName}]");
                nsw.WriteLine($"public class {controllerClassName} : {symbols.ControllerBase.FullName()}");
                nsw.WriteBlock(cb =>
                {
                    cb.WriteLine($"private readonly {serviceFullname} _service;");
                    cb.WriteLine($"public {controllerClassName}({serviceFullname} service)");
                    cb.WriteBlock(mb =>
                    {
                        mb.WriteLine("_service = service;");
                    });
                    
                    foreach (var action in requests.AsActions(context))
                    {
                        cb.WriteLine($"/// <inheritdoc cref=\"{action.ModelTypeName}\"/>");
                        cb.WriteLine($"[{action.HttpMethodAttribute}, {action.HttpRouteAttribute}]");
                        cb.WriteLine($"public {action.ResponseTypeName} {action.ActionName}([{action.ModelFromAttributeName}] {action.ModelTypeName} model, {symbols.CancellationToken.FullName()} ct)");
                        cb.WriteBlock(mb =>
                        {
                            mb.WriteLine($"return _service.{action.Request.GetMethodName()}(model, ct);");
                        });
                    }
                });
            });

            return builder.ToString();
        }
    }
}
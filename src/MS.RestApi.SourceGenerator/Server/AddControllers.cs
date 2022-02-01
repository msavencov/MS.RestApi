using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using MS.RestApi.Abstractions;
using MS.RestApi.Generators.Builder;
using MS.RestApi.Generators.Common;
using MS.RestApi.Generators.Extensions;
using MS.RestApi.Generators.Pipe;
using MS.RestApi.Generators.Utils;

namespace MS.RestApi.Generators.Server
{
    internal class AddControllers : IMiddleware<ApiGenContext>
    {
        public void Execute(ApiGenContext context)
        {
            if (context.Requests.Count == 0)
            {
                throw new ApiGenException(10, "No any request found.");
            }
            
            var query = from request in context.Requests
                        group request by request.EndPoint.Service into actions
                        select new
                        {
                            ServiceName = actions.Key,
                            Actions = actions
                        };
            
            foreach (var item in query)
            {
                var service = new ApiGenSourceCode
                {
                    Name = $"ApiClient.{context.Config.ApiName}.Abstractions.{item.ServiceName}.cs",
                    Source = BuildServiceSourceCode(context, item.ServiceName, item.Actions),
                };
                var controller = new ApiGenSourceCode
                {
                    Name = $"ApiClient.{context.Config.ApiName}.Controllers.{item.ServiceName}Controller.cs",
                    Source = BuildControllerSourceCode(context, item.ServiceName, item.Actions),
                };
                context.SourceCode.Add(service);
                context.SourceCode.Add(controller);
            }
        }

        private string BuildServiceSourceCode(ApiGenContext context, string serviceName, IEnumerable<ApiGenRequest> actions)
        {
            var builder = new StringBuilder();
            var writer = new IndentedWriter(builder, 0);
            
            var config = context.Config;
            var symbol = context.KnownSymbols;

            writer.WriteLine($"#pragma warning disable CS1591");
            writer.WriteLine($"namespace {context.Config.ControllerServiceNamespace}");
            writer.WriteBlock(nsw =>
            {
                nsw.WriteLine($"public interface {ApiGenSymbols.GetServiceInterfaceName(serviceName)} : {symbol.IApiService.FullName()}");
                nsw.WriteBlock(cw =>
                {
                    foreach (var request in actions)
                    {
                        var returnResult = request.GetResponseTypeName(context);
                        var name = request.GetMethodName();
                        var model = request.Request.FullName();
                        var ct = symbol.CancellationToken.FullName();
                        var method = $"{returnResult} {name}({model} model, {ct} ct = default);";

                        cw.WriteLine(method);
                    }
                });
            });
            
            return builder.ToString();
        }

        private string BuildControllerSourceCode(ApiGenContext context, string serviceName, IEnumerable<ApiGenRequest> actions)
        {
            var builder = new StringBuilder();
            var writer = new IndentedWriter(builder, 0);
            var symbols = context.KnownSymbols;
            
            var controllerClassName = $"{serviceName}Controller";
            var apiControllerName = symbols.ApiControllerAttribute.FullName();
            var routeAttributeName = symbols.RouteAttribute.FullName();
            var serviceFullname = $"{context.Config.ControllerServiceNamespace}.{ApiGenSymbols.GetServiceInterfaceName(serviceName)}";
            writer.WriteLine("#pragma warning disable CS1591");
            writer.WriteLine($"namespace {context.Config.ControllerNamespace}");
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
                    
                    foreach (var action in actions)
                    {
                        var methodAttributeName = (action.EndPoint.Method switch
                        {
                            Method.Delete => symbols.HttpDeleteAttribute,
                            Method.Post => symbols.HttpPostAttribute,
                            _ => symbols.HttpGetAttribute,
                        }).FullName();
                        
                        var responseTypeName = action.Response.FullName();
                        var methodResponseName = action.GetResponseTypeName(context);
                        var responseTypeIsVoid = SymbolEqualityComparer.Default.Equals(action.Response, symbols.Task);
                        var route = action.GetEndpointRoute(context.Config).Quote();
                        var actionName = action.Request.Name;
                        var fromAttributeName = action.EndPoint.Method switch
                        {
                            Method.Post => symbols.FromBodyAttribute.FullName(),
                            _ => symbols.FromQueryAttribute.FullName(),
                        };
                        var requestTypeName = action.Request.FullName();
                        var cancellationTokenName = symbols.CancellationToken.FullName();
                        
                        cb.WriteLine("/// <summary>");
                        cb.WriteLine("/// Test comment");
                        cb.WriteLine("/// </summary>");
                        cb.WriteLine($"[{methodAttributeName}]");
                        cb.WriteLine($"[{routeAttributeName}({route})]");
                        cb.WriteLine($"public {methodResponseName} {actionName}([{fromAttributeName}] {requestTypeName} model, {cancellationTokenName} ct)");
                        cb.WriteBlock(mb =>
                        {
                            mb.WriteLine($"return _service.{action.GetMethodName()}(model, ct);");
                        });
                    }
                });
            });

            return builder.ToString();
        }
    }
}
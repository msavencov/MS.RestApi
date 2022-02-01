using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using MS.RestApi.Generators.Builder;
using MS.RestApi.Generators.Common;
using MS.RestApi.Generators.Extensions;
using MS.RestApi.Generators.Pipe;
using MS.RestApi.Generators.Utils;

namespace MS.RestApi.Generators.Client
{
    internal class AddClientImplementation : IMiddleware<ApiGenContext>
    {
        public void Execute(ApiGenContext context)
        {
            var services = from request in context.Requests
                           group request by request.EndPoint.Service into rg
                           select new
                           {
                               Group = rg.Key,
                               Actions = rg.AsEnumerable()
                           };
            var config = context.Config;
            var symbol = context.KnownSymbols;

            var symbolComparer = SymbolEqualityComparer.Default;
            var requestHandlerInterface = ApiGenSymbols.GetClientRequestHandlerInterface(symbol, config);
            var requestHandlerInterfaceName = $"{requestHandlerInterface.ContainingNamespace}.{requestHandlerInterface.InterfaceName}";
            var methodName = symbol.Method.FullName();
            
            foreach (var service in services)
            {
                var builder = new StringBuilder();
                var writer = new IndentedWriter(builder, 0);

                var clientApiName = ApiGenRequest.BuildClientName(service.Group);
                var clientInterfaceName = ApiGenRequest.BuildInterfaceName(service.Group);
                var clientInterfaceFullName = $"{config.ClientInterfaceNamespace}.{clientInterfaceName}";
                var cancellationTokenType = symbol.CancellationToken.FullName();
                
                writer.WriteLine($"namespace {config.ClientImplementationNamespace}");
                writer.WriteBlock(nsw =>
                {
                    nsw.WriteLine($"internal class {clientApiName} : {clientInterfaceFullName}");
                    nsw.WriteBlock(cw =>
                    {
                        cw.WriteLine($"private readonly {requestHandlerInterfaceName} _httpRequestHandler;");
                        cw.WriteLine();
                        cw.WriteLine($"public {clientApiName}({requestHandlerInterfaceName} httpRequestHandler)");
                        cw.WriteBlock(mw =>
                        {
                            mw.WriteLine("_httpRequestHandler = httpRequestHandler;");
                        });

                        foreach (var action in service.Actions)
                        {
                            var responseType = action.GetResponseTypeName(context);
                            var requestType = action.Request.FullName();

                            cw.WriteLine();
                            cw.WriteLine($"public {responseType} {action.GetMethodName()}({requestType} model, {cancellationTokenType} ct = default)");
                            cw.WriteBlock(mw =>
                            {
                                var handlerTypeArgs = action.Request.FullName();

                                if (symbolComparer.Equals(action.Response, symbol.Task) == false)
                                {
                                    handlerTypeArgs += $", {action.Response.FullName()}";
                                }

                                var argMethod = $"{methodName}.{action.EndPoint.Method}";
                                var argResource = action.GetEndpointRoute(config).Quote();

                                mw.WriteLine($"return _httpRequestHandler.HandleAsync<{handlerTypeArgs}>({argMethod}, {argResource}, model, ct);");
                            });
                        }
                    });
                });
                
                var sourceCode = new ApiGenSourceCode
                {
                    Name = $"ApiClient.{config.ApiName}.Impl.{service.Group}.cs",
                    Source = builder.ToString()
                };
                
                context.SourceCode.Add(sourceCode);
            }
        }
    }
}
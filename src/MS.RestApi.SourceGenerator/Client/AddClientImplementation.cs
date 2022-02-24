using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using MS.RestApi.SourceGenerator.Builder;
using MS.RestApi.SourceGenerator.Common;
using MS.RestApi.SourceGenerator.Extensions;
using MS.RestApi.SourceGenerator.Pipe;
using MS.RestApi.SourceGenerator.Utils;

namespace MS.RestApi.SourceGenerator.Client
{
    internal class AddClientImplementation : IMiddleware<ApiGenContext>
    {
        public void Execute(ApiGenContext context)
        {
            var config = context.Config;
            var symbol = context.KnownSymbols;
            var services = context.Requests.AsServices();

            var symbolComparer = SymbolEqualityComparer.Default;
            var requestHandlerInterface = ApiGenSymbols.GetClientRequestHandlerInterface(symbol, config);
            var requestHandlerInterfaceName = $"{requestHandlerInterface.ContainingNamespace}.{requestHandlerInterface.InterfaceName}";
            var methodName = symbol.Method.FullName();
            
            foreach (var service in services)
            {
                var builder = new StringBuilder();
                var writer = new IndentedWriter(builder, 0);

                var clientApiName = ApiGenRequest.BuildClientName(service.ServiceName);
                var clientInterfaceName = ApiGenRequest.BuildInterfaceName(service.ServiceName);
                var clientInterfaceFullName = $"{config.ClientServicesNamespace}.{clientInterfaceName}";
                var cancellationTokenType = symbol.CancellationToken.FullName();
                
                writer.WriteLine($"namespace {config.ClientServicesImplNamespace}");
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

                        foreach (var action in service.Operations)
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
                    Name = $"{config.ClientServicesImplNamespace}.{service.ServiceName}.cs",
                    Source = builder.ToString()
                };
                
                context.SourceCode.Add(sourceCode);
            }
        }
    }
}
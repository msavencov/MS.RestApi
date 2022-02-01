using System.Collections.Generic;
using System.Linq;
using System.Text;
using MS.RestApi.Generators.Builder;
using MS.RestApi.Generators.Common;
using MS.RestApi.Generators.Extensions;
using MS.RestApi.Generators.Pipe;
using MS.RestApi.Generators.Utils;

namespace MS.RestApi.Generators.Client
{
    internal class AddClientInterfaces : IMiddleware<ApiGenContext>
    {
        public void Execute(ApiGenContext context)
        {
            if (context.Requests.Count == 0)
            {
                throw new ApiGenException(10, "No any request found.");
            }

            BuildInterfaceSourceCode(context);
            BuildHandlerInterface(context);
        }

        private void BuildHandlerInterface(ApiGenContext context)
        {
            var config = context.Config;
            var symbols = context.KnownSymbols;
            var apiName = config.ApiName;

            var builder = new StringBuilder();
            var writer = new IndentedWriter(builder);

            var (interfaceName, containingNamespace) = ApiGenSymbols.GetClientRequestHandlerInterface(symbols, config);
            var baseRequestHandlerInterfaceName = symbols.ClientRequestHandlerInterface.FullName();
            
            writer.WriteLine($"namespace {containingNamespace}");
            writer.WriteBlock(nsw =>
            {
                nsw.WriteLine($"public interface {interfaceName} : {baseRequestHandlerInterfaceName}");
                nsw.WriteBlock(_ => { });
            });
            
            context.SourceCode.Add(new ApiGenSourceCode
            {
                Name = $"ApiClient.{apiName}.Abstractions.{interfaceName}.cs",
                Source = builder.ToString()
            });
        }

        private void BuildInterfaceSourceCode(ApiGenContext context)
        {
            var query = from request in context.Requests
                        group request by request.EndPoint.Service into actions
                        select new
                        {
                            GroupName = actions.Key,
                            Actions = actions
                        };

            foreach (var service in query)
            {
                var sourceCode = new ApiGenSourceCode
                {
                    Name = $"ApiClient.{context.Config.ApiName}.Abstractions.{service.GroupName}.cs",
                    Source = BuildInterfaceSourceCode(service.GroupName, service.Actions, context),
                };
                
                context.SourceCode.Add(sourceCode);
            }
        }
        
        private string BuildInterfaceSourceCode(string group, IEnumerable<ApiGenRequest> actions, ApiGenContext context)
        {
            var builder = new StringBuilder();
            var writer = new IndentedWriter(builder, 0);
            
            var config = context.Config;
            var symbol = context.KnownSymbols;
            
            writer.WriteLine($"namespace {config.ClientInterfaceNamespace}");
            writer.WriteBlock(namespaceBuilder =>
            {
                namespaceBuilder.WriteLine($"public interface {ApiGenRequest.BuildInterfaceName(group)} : {symbol.IApiService.FullName()}");
                namespaceBuilder.WriteBlock(interfaceBuilder =>
                {
                    foreach (var request in actions)
                    {
                        var returnResult = request.GetResponseTypeName(context);
                        var name = request.GetMethodName();
                        var model = request.Request.FullName();
                        var ct = symbol.CancellationToken.FullName();
                        var method = $"{returnResult} {name}({model} model, {ct} ct = default);";

                        interfaceBuilder.WriteLine(method);
                    }
                });
            });

            return builder.ToString();
        }
    }
}
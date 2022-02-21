using System.Collections.Generic;
using System.Linq;
using System.Text;
using MS.RestApi.SourceGenerator.Builder;
using MS.RestApi.SourceGenerator.Common;
using MS.RestApi.SourceGenerator.Extensions;
using MS.RestApi.SourceGenerator.Pipe;
using MS.RestApi.SourceGenerator.Utils;

namespace MS.RestApi.SourceGenerator.Client
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
                Name = $"{config.ClientRootNamespace}.{interfaceName}.cs",
                Source = builder.ToString()
            });
        }

        private void BuildInterfaceSourceCode(ApiGenContext context)
        {
            foreach (var service in context.Requests.AsServices())
            {
                var sourceCode = new ApiGenSourceCode
                {
                    Name = $"{context.Config.ClientServicesNamespace}.{service.ServiceName}.cs",
                    Source = BuildInterfaceSourceCode(service.ServiceName, service.Operations, context),
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
            
            writer.WriteLine($"namespace {config.ClientRootNamespace}");
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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using MS.RestApi.Abstractions;
using MS.RestApi.Generators.Extensions;

namespace MS.RestApi.Generators.Utils
{
    internal class ApiGenSymbols
    {
        public ISymbol IApiService {get; private set;}
        public ISymbol EndPointAttribute {get; private set;}
        public ISymbol ClientRequestHandlerInterface {get; private set;}
        public ISymbol Request {get; private set;}
        public ISymbol Task {get; private set;}
        public ISymbol TaskGeneric {get; private set;}
        public ISymbol CancellationToken {get; private set;}
        public ISymbol ControllerBase {get; private set;}
        public ISymbol Method { get; private set; }
        public ISymbol ApiControllerAttribute { get; private set; }
        public ISymbol FromBodyAttribute { get; private set; }
        public ISymbol FromQueryAttribute { get; private set; }
        public ISymbol HttpGetAttribute { get; private set; }
        public ISymbol HttpPostAttribute { get; private set; }
        public ISymbol HttpDeleteAttribute { get; private set; }
        public ISymbol RouteAttribute { get; private set; }
        public ISymbol IServiceCollection { get; private set; }
        public ISymbol Action { get; private set; }
        public ISymbol ArgumentNullException { get; private set; }
        public ISymbol ServiceDescriptor { get; private set; }
        public ISymbol ServiceLifetime { get; private set; }

        public static ApiGenSymbols Init(GeneratorExecutionContext context)
        {
            var symbols = new ApiGenSymbols
            {
                Method = context.Compilation.GetTypeByMetadataName("MS.RestApi.Abstractions.Method"),
                IApiService = context.Compilation.GetTypeByMetadataName("MS.RestApi.Abstractions.IApiService"),
                EndPointAttribute = context.Compilation.GetTypeByMetadataName("MS.RestApi.Abstractions.EndPointAttribute"),
                Request = context.Compilation.GetTypeByMetadataName("MS.RestApi.Abstractions.Request"),
                
                CancellationToken = context.Compilation.GetTypeByMetadataName("System.Threading.CancellationToken"),
                Task = context.Compilation.GetTypeByMetadataName("System.Threading.Tasks.Task"),
                TaskGeneric = context.Compilation.GetTypeByMetadataName("System.Threading.Tasks.Task`1"),
                ControllerBase = context.Compilation.GetTypeByMetadataName("Microsoft.AspNetCore.Mvc.ControllerBase"),
                ApiControllerAttribute = context.Compilation.GetTypeByMetadataName("Microsoft.AspNetCore.Mvc.ApiControllerAttribute"),
                FromBodyAttribute = context.Compilation.GetTypeByMetadataName("Microsoft.AspNetCore.Mvc.FromBodyAttribute"),
                FromQueryAttribute = context.Compilation.GetTypeByMetadataName("Microsoft.AspNetCore.Mvc.FromQueryAttribute"),
                HttpGetAttribute = context.Compilation.GetTypeByMetadataName("Microsoft.AspNetCore.Mvc.HttpGetAttribute"),
                HttpPostAttribute = context.Compilation.GetTypeByMetadataName("Microsoft.AspNetCore.Mvc.HttpPostAttribute"),
                HttpDeleteAttribute = context.Compilation.GetTypeByMetadataName("Microsoft.AspNetCore.Mvc.HttpDeleteAttribute"),
                RouteAttribute = context.Compilation.GetTypeByMetadataName("Microsoft.AspNetCore.Mvc.RouteAttribute"),
                IServiceCollection = context.Compilation.GetTypeByMetadataName("Microsoft.Extensions.DependencyInjection.IServiceCollection"),
                Action = context.Compilation.GetTypeByMetadataName("System.Action"),
                ArgumentNullException = context.Compilation.GetTypeByMetadataName("System.ArgumentNullException"),
                ServiceDescriptor = context.Compilation.GetTypeByMetadataName("Microsoft.Extensions.DependencyInjection.ServiceDescriptor"),
                ServiceLifetime = context.Compilation.GetTypeByMetadataName("Microsoft.Extensions.DependencyInjection.ServiceLifetime"),
            };
            
            return symbols;
        }

        public static (string InterfaceName, string ContainingNamespace) GetClientRequestHandlerInterface(ApiGenSymbols symbols, ApiGenConfig config)
        {
            var requestHandlerInterfaceName = symbols.ClientRequestHandlerInterface.Name.Insert(1, config.ApiName);
            var requestHandlerInterfaceNamespace = config.ClientInterfaceNamespace;

            return (requestHandlerInterfaceName, requestHandlerInterfaceNamespace);

        }

        public static string GetServiceInterfaceName(string serviceName)
        {
            return $"I{serviceName}Service";
        }
    }
}
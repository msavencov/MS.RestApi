using Microsoft.CodeAnalysis;

namespace MS.RestApi.SourceGenerator.Utils
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
        public ISymbol ApiControllerAttribute { get; private set; }
        public ISymbol FromBodyAttribute { get; private set; }
        public ISymbol FromQueryAttribute { get; private set; }
        public ISymbol HttpPostAttribute { get; private set; }
        public ISymbol RouteAttribute { get; private set; }
        public ISymbol IServiceCollection { get; private set; }
        public ISymbol Action { get; private set; }
        public ISymbol ArgumentNullException { get; private set; }
        public ISymbol ServiceDescriptor { get; private set; }
        public ISymbol ServiceLifetime { get; private set; }
        public ISymbol Mediator { get; private set; }
        
        public static ApiGenSymbols Init(GeneratorExecutionContext context)
        {
            var symbols = new ApiGenSymbols
            {
                IApiService = context.Compilation.GetTypeByMetadataName("MS.RestApi.Abstractions.IApiService"),
                EndPointAttribute = context.Compilation.GetTypeByMetadataName("MS.RestApi.Abstractions.EndPointAttribute"),
                Request = context.Compilation.GetTypeByMetadataName("MS.RestApi.Abstractions.Request"),
                
                ClientRequestHandlerInterface = context.Compilation.GetTypeByMetadataName("MS.RestApi.Client.IRequestHandler"),
                CancellationToken = context.Compilation.GetTypeByMetadataName("System.Threading.CancellationToken"),
                Task = context.Compilation.GetTypeByMetadataName("System.Threading.Tasks.Task"),
                TaskGeneric = context.Compilation.GetTypeByMetadataName("System.Threading.Tasks.Task`1"),
                ControllerBase = context.Compilation.GetTypeByMetadataName("Microsoft.AspNetCore.Mvc.ControllerBase"),
                ApiControllerAttribute = context.Compilation.GetTypeByMetadataName("Microsoft.AspNetCore.Mvc.ApiControllerAttribute"),
                FromBodyAttribute = context.Compilation.GetTypeByMetadataName("Microsoft.AspNetCore.Mvc.FromBodyAttribute"),
                FromQueryAttribute = context.Compilation.GetTypeByMetadataName("Microsoft.AspNetCore.Mvc.FromQueryAttribute"),
                HttpPostAttribute = context.Compilation.GetTypeByMetadataName("Microsoft.AspNetCore.Mvc.HttpPostAttribute"),
                RouteAttribute = context.Compilation.GetTypeByMetadataName("Microsoft.AspNetCore.Mvc.RouteAttribute"),
                IServiceCollection = context.Compilation.GetTypeByMetadataName("Microsoft.Extensions.DependencyInjection.IServiceCollection"),
                Action = context.Compilation.GetTypeByMetadataName("System.Action"),
                ArgumentNullException = context.Compilation.GetTypeByMetadataName("System.ArgumentNullException"),
                ServiceDescriptor = context.Compilation.GetTypeByMetadataName("Microsoft.Extensions.DependencyInjection.ServiceDescriptor"),
                ServiceLifetime = context.Compilation.GetTypeByMetadataName("Microsoft.Extensions.DependencyInjection.ServiceLifetime"),
                Mediator = context.Compilation.GetTypeByMetadataName("MediatR.IMediator")
            };
            
            return symbols;
        }

        public static (string InterfaceName, string ContainingNamespace) GetClientRequestHandlerInterface(ApiGenSymbols symbols, ApiGenConfig config)
        {
            var requestHandlerInterfaceName = symbols.ClientRequestHandlerInterface.Name.Insert(1, config.ApiName);
            var requestHandlerInterfaceNamespace = config.ClientRootNamespace;

            return (requestHandlerInterfaceName, requestHandlerInterfaceNamespace);

        }

        public static string GetServiceInterfaceName(string serviceName)
        {
            return $"I{serviceName}Service";
        }
    }
}
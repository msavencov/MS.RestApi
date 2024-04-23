using System.Collections.Concurrent;
using Microsoft.CodeAnalysis;
using MS.RestApi.SourceGenerator.Descriptors;

namespace MS.RestApi.SourceGenerator.Generators;

internal class KnownSymbols
{
    private KnownSymbols(Compilation compilation)
    {
        ApiGenOptionsAttribute = compilation.GetTypeByMetadataName(Names.ApiGenConfigAttribute)!;
        IApiService = compilation.GetTypeByMetadataName(Names.IApiService)!;
        EndPointAttribute = compilation.GetTypeByMetadataName(Names.EndPointAttribute)!;
        Request = compilation.GetTypeByMetadataName(Names.Request)!;

        ClientRequestHandlerInterface = compilation.GetTypeByMetadataName("MS.RestApi.Client.IRequestHandler")!;
        CancellationToken = compilation.GetTypeByMetadataName("System.Threading.CancellationToken")!;
        Task = compilation.GetTypeByMetadataName("System.Threading.Tasks.Task")!;
        TaskGeneric = compilation.GetTypeByMetadataName("System.Threading.Tasks.Task`1")!;
        ControllerBase = compilation.GetTypeByMetadataName("Microsoft.AspNetCore.Mvc.ControllerBase")!;
        ApiControllerAttribute = compilation.GetTypeByMetadataName("Microsoft.AspNetCore.Mvc.ApiControllerAttribute")!;
        FromBodyAttribute = compilation.GetTypeByMetadataName("Microsoft.AspNetCore.Mvc.FromBodyAttribute")!;
        FromQueryAttribute = compilation.GetTypeByMetadataName("Microsoft.AspNetCore.Mvc.FromQueryAttribute")!;
        HttpPostAttribute = compilation.GetTypeByMetadataName("Microsoft.AspNetCore.Mvc.HttpPostAttribute")!;
        RouteAttribute = compilation.GetTypeByMetadataName("Microsoft.AspNetCore.Mvc.RouteAttribute")!;
        IServiceCollection = compilation.GetTypeByMetadataName("Microsoft.Extensions.DependencyInjection.IServiceCollection")!;
        Action = compilation.GetTypeByMetadataName("System.Action")!;
        ArgumentNullException = compilation.GetTypeByMetadataName("System.ArgumentNullException")!;
        ServiceDescriptor = compilation.GetTypeByMetadataName("Microsoft.Extensions.DependencyInjection.ServiceDescriptor")!;
        ServiceLifetime = compilation.GetTypeByMetadataName("Microsoft.Extensions.DependencyInjection.ServiceLifetime")!;
        Mediator = compilation.GetTypeByMetadataName("MediatR.IMediator");
    }
    
    public INamedTypeSymbol ApiGenOptionsAttribute { get; set; }
    public INamedTypeSymbol Request {get; private set;}
    
    // ReSharper disable once InconsistentNaming
    public INamedTypeSymbol IApiService { get; private set; }
    public INamedTypeSymbol EndPointAttribute {get; private set;}
    
    public INamedTypeSymbol ClientRequestHandlerInterface {get; private set;} 
    
    public INamedTypeSymbol ControllerBase {get; private set;}
    public INamedTypeSymbol ApiControllerAttribute { get; private set; }
    public INamedTypeSymbol RouteAttribute { get; private set; }
    public INamedTypeSymbol FromBodyAttribute { get; private set; }
    public INamedTypeSymbol FromQueryAttribute { get; private set; }
    public INamedTypeSymbol HttpPostAttribute { get; private set; }
    
    public INamedTypeSymbol Action { get; private set; } 
    public INamedTypeSymbol Task {get; private set;} 
    public INamedTypeSymbol TaskGeneric {get; private set;} 
    public INamedTypeSymbol CancellationToken {get; private set;} 
    public INamedTypeSymbol ArgumentNullException { get; private set; } 
    // ReSharper disable once InconsistentNaming
    public INamedTypeSymbol IServiceCollection { get; private set; } 
    public INamedTypeSymbol ServiceDescriptor { get; private set; } 
    public INamedTypeSymbol ServiceLifetime { get; private set; }

    public ISymbol? Mediator { get; private set; }

    private static readonly ConcurrentDictionary<Compilation, KnownSymbols> SymbolCache = new();
    public static KnownSymbols FromCompilation(Compilation compilation) => SymbolCache.GetOrAdd(compilation, c => new KnownSymbols(c));

    public static (string InterfaceName, string ContainingNamespace) GetClientRequestHandlerInterface(KnownSymbols symbols, ApiGenOptions config)
    {
        var requestHandlerInterfaceName = symbols.ClientRequestHandlerInterface.Name.Insert(1, config.ApiName);
        var requestHandlerInterfaceNamespace = config.ClientConventions.RootNamespace;

        return (requestHandlerInterfaceName, requestHandlerInterfaceNamespace);

    }

    internal static class Names
    {
        public const string IApiService = "MS.RestApi.Abstractions.IApiService";
        public const string EndPointAttribute = "MS.RestApi.Abstractions.EndPointAttribute";
        public const string ApiGenConfigAttribute = "MS.RestApi.ApiGenOptionsAttribute";
        public const string Request = "MS.RestApi.Abstractions.Request";
    }
}
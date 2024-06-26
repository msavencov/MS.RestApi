﻿using Microsoft.CodeAnalysis;

// ReSharper disable InconsistentNaming

namespace MS.RestApi.SourceGenerator.Generators;

internal class KnownSymbols(Compilation compilation)
{
    #nullable disable
    
    public readonly INamedTypeSymbol ApiGenOptionsAttribute = compilation.GetTypeByMetadataName("MS.RestApi.ApiGenOptionsAttribute");
    public readonly INamedTypeSymbol IApiRequest = compilation.GetTypeByMetadataName("MS.RestApi.Abstractions.IApiRequest");
    public readonly INamedTypeSymbol IApiRequest1 = compilation.GetTypeByMetadataName("MS.RestApi.Abstractions.IApiRequest`1");

    public readonly INamedTypeSymbol IApiService = compilation.GetTypeByMetadataName("MS.RestApi.Abstractions.IApiService");
    public readonly INamedTypeSymbol EndPointAttribute = compilation.GetTypeByMetadataName("MS.RestApi.Abstractions.EndPointAttribute");

    public readonly INamedTypeSymbol IRequestHandler = compilation.GetTypeByMetadataName("MS.RestApi.Client.IRequestHandler");

    public readonly INamedTypeSymbol ControllerBase = compilation.GetTypeByMetadataName("Microsoft.AspNetCore.Mvc.ControllerBase");
    public readonly INamedTypeSymbol ApiControllerAttribute = compilation.GetTypeByMetadataName("Microsoft.AspNetCore.Mvc.ApiControllerAttribute");
    public readonly INamedTypeSymbol RouteAttribute = compilation.GetTypeByMetadataName("Microsoft.AspNetCore.Mvc.RouteAttribute");
    public readonly INamedTypeSymbol FromBodyAttribute = compilation.GetTypeByMetadataName("Microsoft.AspNetCore.Mvc.FromBodyAttribute");
    public readonly INamedTypeSymbol FromQueryAttribute = compilation.GetTypeByMetadataName("Microsoft.AspNetCore.Mvc.FromQueryAttribute");
    public readonly INamedTypeSymbol FromRouteAttribute = compilation.GetTypeByMetadataName("Microsoft.AspNetCore.Mvc.FromRouteAttribute");
    public readonly INamedTypeSymbol HttpPostAttribute = compilation.GetTypeByMetadataName("Microsoft.AspNetCore.Mvc.HttpPostAttribute");

    public readonly INamedTypeSymbol Action = compilation.GetTypeByMetadataName("System.Action");
    public readonly INamedTypeSymbol Task = compilation.GetTypeByMetadataName(KnownSymbolNames.Task);
    public readonly INamedTypeSymbol TaskGeneric = compilation.GetTypeByMetadataName($"{KnownSymbolNames.Task}`1");
    public readonly INamedTypeSymbol CancellationToken = compilation.GetTypeByMetadataName("System.Threading.CancellationToken");
    public readonly INamedTypeSymbol ArgumentNullException = compilation.GetTypeByMetadataName("System.ArgumentNullException");

    public readonly INamedTypeSymbol IServiceCollection = compilation.GetTypeByMetadataName("Microsoft.Extensions.DependencyInjection.IServiceCollection");
    public readonly INamedTypeSymbol ServiceDescriptor = compilation.GetTypeByMetadataName("Microsoft.Extensions.DependencyInjection.ServiceDescriptor");
    public readonly INamedTypeSymbol ServiceLifetime = compilation.GetTypeByMetadataName("Microsoft.Extensions.DependencyInjection.ServiceLifetime");

    #nullable restore
    
    public readonly ISymbol? IMediator = compilation.GetTypeByMetadataName("MediatR.IMediator");
    public readonly INamedTypeSymbol MediatorRequest =  compilation.GetTypeByMetadataName("MediatR.IRequest")!;
    public readonly INamedTypeSymbol MediatorRequest1 =  compilation.GetTypeByMetadataName("MediatR.IRequest`1")?.ConstructUnboundGenericType()!;
    public readonly INamedTypeSymbol MediatorRequestHandler1 = compilation.GetTypeByMetadataName("MediatR.IRequestHandler`1")!;
    public readonly INamedTypeSymbol MediatorRequestHandler2 = compilation.GetTypeByMetadataName("MediatR.IRequestHandler`2")!;
}

internal static class KnownSymbolNames
{
    public const string Task = "System.Threading.Tasks.Task";
}
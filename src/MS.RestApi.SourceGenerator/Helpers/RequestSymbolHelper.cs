using System.Linq;
using Microsoft.CodeAnalysis;
using MS.RestApi.SourceGenerator.Generators;

namespace MS.RestApi.SourceGenerator.Helpers;

internal class RequestSymbolHelper(KnownSymbols symbols)
{
    public (bool Valid, INamedTypeSymbol? Response) ValidateRequest(INamedTypeSymbol request)
    {
        if (request.FindGenericInterface(symbols.IApiRequest1).SingleOrDefault() is { } ar)
        {
            return (true, (INamedTypeSymbol)ar.TypeArguments.Single());
        }
        
        if (request.FindInterface(symbols.IApiRequest).SingleOrDefault() is not null)
        {
            return (true, default);
        }
        
        if (request.FindGenericInterface(symbols.MediatorRequest1).SingleOrDefault() is { } @interface)
        {
            return (true, (INamedTypeSymbol?)@interface.TypeArguments.Single());
        }

        if (request.FindInterface(symbols.MediatorRequest).SingleOrDefault() is not null)
        {
            return (true, default);
        }

        return (false, default);
    }    
}
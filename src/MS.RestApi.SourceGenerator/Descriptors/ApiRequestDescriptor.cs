using Microsoft.CodeAnalysis;
using MS.RestApi.SourceGenerator.Generators;

namespace MS.RestApi.SourceGenerator.Descriptors;

internal record ApiRequestDescriptor
{
    public required string Service { get; init; }
    public required string Endpoint { get; init; }
    
    public required INamedTypeSymbol Request { get; init; } 
    public required INamedTypeSymbol? Response { get; init; }
    public string ReturnType => Response is null ? KnownSymbolNames.Task : $"{KnownSymbolNames.Task}<{Response.ToDisplayString()}>";
}
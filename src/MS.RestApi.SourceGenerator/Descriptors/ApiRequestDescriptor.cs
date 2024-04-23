using Microsoft.CodeAnalysis;

namespace MS.RestApi.SourceGenerator.Descriptors;

internal record ApiRequestDescriptor
{
    public required string Service { get; init; }
    public required string Endpoint { get; init; }
    
    public required INamedTypeSymbol Request { get; init; } 
    public required INamedTypeSymbol? Response { get; init; }
    
    public string GetResponseTypeName(INamedTypeSymbol task)
    {
        var responseType = task.ToDisplayString();
                
        if (Response is not null)
        {
            responseType = $"{responseType}<{Response.ToDisplayString()}>";
        }

        return responseType;
    }
}
using Microsoft.CodeAnalysis;
using MS.RestApi.SourceGenerator.Generators;

namespace MS.RestApi.SourceGenerator.Tests.Helpers;

internal class TestSymbols(Compilation compilation) : KnownSymbols(compilation)
{
    public readonly SymbolEqualityComparer Comparer = SymbolEqualityComparer.Default;
    public readonly INamedTypeSymbol Request1 = compilation.GetTypeByMetadataName("Templates.Request1")!;
    public readonly INamedTypeSymbol Request2 = compilation.GetTypeByMetadataName("Templates.Request2")!;
    public readonly INamedTypeSymbol Request2Response = compilation.GetTypeByMetadataName("Templates.Request2Response")!;
}
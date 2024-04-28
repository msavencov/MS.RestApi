using Microsoft.CodeAnalysis;
using MS.RestApi.SourceGenerator.Generators;
using MS.RestApi.SourceGenerator.Helpers;

namespace MS.RestApi.SourceGenerator.Tests.Helpers;

internal class AssertSymbolHelper(KnownSymbols symbols)
{
    private readonly RequestSymbolHelper _helper = new(symbols);
    public IMethodSymbol AssertRequestMethod(List<IMethodSymbol> methods, INamedTypeSymbol request)
    {
        var method = methods.Single(t => t.Name == request.Name);
        var comparer = SymbolEqualityComparer.Default;
        var (valid, response) = _helper.ValidateRequest(request);
        
        Assert.True(valid);
        
        var result = response is {} ? symbols.TaskGeneric.Construct(response) : symbols.Task;
        
        Assert.Equal(2, method.Parameters.Length);
        Assert.Equal(method.Parameters[0].Type, request, comparer);
        Assert.Equal(method.Parameters[1].Type, symbols.CancellationToken, comparer);
        Assert.Equal(method.ReturnType, result, comparer);

        return method;
    }
}
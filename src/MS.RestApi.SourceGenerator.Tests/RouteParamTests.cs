using Microsoft.CodeAnalysis;
using MS.RestApi.SourceGenerator.Extensions;
using MS.RestApi.SourceGenerator.Generators;
using MS.RestApi.SourceGenerator.Tests.Helpers;

namespace MS.RestApi.SourceGenerator.Tests;

public class RouteParamTests
{
    [Fact]
    public void Default()
    {
        // arrange
        var compilation = CreateCompilation();
        
        // act
        var result = CompilationFactory.CreateAndRunGenerators<ApiGenerator>(compilation);
        
        // assert 
        var symbols = new TestSymbols(result);
        var comparer = SymbolEqualityComparer.Default;
        
        Assert.Empty(result.GetDiagnostics());
    }
    
    private static Compilation CreateCompilation()
    {
        var assembly = typeof(RouteParamTests).Assembly;
        var options = assembly.ReadEmbeddedResource("RouteParam/Options.cs");
        var requests = new[] { "RouteParam/Request.cs" }.Select(assembly.ReadEmbeddedResource);
        
        var contract = CompilationFactory.CreateContractAssembly(requests);
        var compilation = CompilationFactory.CreateCompilation(contract, options);

        return compilation;
    }
    
    private class TestSymbols(Compilation compilation) : KnownSymbols(compilation)
    {
        
    }

}
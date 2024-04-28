using System.Reflection;
using Microsoft.CodeAnalysis;
using MS.RestApi.SourceGenerator.Extensions;
using MS.RestApi.SourceGenerator.Generators;
using MS.RestApi.SourceGenerator.Tests.Helpers;

namespace MS.RestApi.SourceGenerator.Tests;

public class WithMediatorTests
{
    private static readonly Assembly Assembly = typeof(WithServicesTests).Assembly;

    [Fact]
    public void Generate_ServicesWithMediator()
    {
        // arrange
        var options = Assembly.ReadEmbeddedResource("Mediator/Options.cs");
        var requests = new[] { "Mediator/Request1.cs", "Mediator/Request2.cs" }.Select(Assembly.ReadEmbeddedResource);
        
        var contract = CompilationFactory.CreateContractAssembly(requests);
        var compilation = CompilationFactory.CreateCompilation(contract, options);
        
        // act
        var result = CompilationFactory.CreateAndRunGenerators<ApiGenerator>(compilation, out var output);
        var symbols = new TestSymbols(output);
        var comparer = SymbolEqualityComparer.Default;
        
        // assert 
        Assert.Empty(result.Diagnostics);
        
        var services = output.GetSymbolsWithName("ITestService", SymbolFilter.Type).OfType<INamedTypeSymbol>();
        var service = Assert.Single(services);

        var handler1 = symbols.MediatorRequestHandler1.Construct(symbols.Request1);
        var handler2 = symbols.MediatorRequestHandler2.Construct(symbols.Request2, symbols.Request2Result);

        Assert.Single(service.Interfaces, t => comparer.Equals(t, handler1));
        Assert.Single(service.Interfaces, t => comparer.Equals(t, handler2));
    }

    private class TestSymbols(Compilation compilation) : KnownSymbols(compilation)
    {
        public readonly INamedTypeSymbol Request1 = compilation.GetTypeByMetadataName("Templates.Mediator.Request1")!;
        public readonly INamedTypeSymbol Request2 = compilation.GetTypeByMetadataName("Templates.Mediator.Request2")!;
        public readonly INamedTypeSymbol Request2Result = compilation.GetTypeByMetadataName("Templates.Mediator.Request2Result")!;
    }
}
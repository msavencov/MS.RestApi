using System.Reflection;
using Microsoft.CodeAnalysis;
using MS.RestApi.SourceGenerator.Extensions;
using MS.RestApi.SourceGenerator.Generators;
using MS.RestApi.SourceGenerator.Tests.Helpers;

namespace MS.RestApi.SourceGenerator.Tests;

public class WithServicesTests 
{
    private static readonly Assembly Assembly = typeof(WithServicesTests).Assembly;
    
    [Fact]
    public void Generate_ControllersWithServicesAndClient()
    {
        // arrange
        var options = Assembly.ReadEmbeddedResource("WithServices.cs");
        var requests = new[] { "Requests/Request1.cs", "Requests/Request2.cs" }.Select(Assembly.ReadEmbeddedResource);

        var contract = CompilationFactory.CreateContractAssembly(requests);
        var compilation = CompilationFactory.CreateCompilation(contract, options);
        
        // act
        var result = CompilationFactory.CreateAndRunGenerators<ApiGenerator>(compilation);
        var symbols = new TestSymbols(result);
        
        // assert 
        
        AssertService(result, symbols);
        AssertController(result, symbols);
        AssertRequestHandler(result, symbols);
        AssertClient(result, symbols);
        AssertClientService(result, symbols);
    }

    private void AssertClientService(Compilation compilation, TestSymbols symbols)
    {
        
    }

    private void AssertClient(Compilation compilation, TestSymbols symbols)
    {
        var clients = compilation.GetSymbolsWithName("GroupApiClient", SymbolFilter.Type).OfType<INamedTypeSymbol>();
        var client = Assert.Single(clients);
        var clientMethods = client.GetMembers().OfType<IMethodSymbol>().ToList();
        var clientInterface = Assert.Single(client.Interfaces);
        var clientInterfaceMethods = clientInterface.GetMembers().OfType<IMethodSymbol>().ToList();
        
        var services = compilation.GetSymbolsWithName("IGroupApi", SymbolFilter.Type).OfType<INamedTypeSymbol>();
        var service = Assert.Single(services);
        var helper = new AssertSymbolHelper(symbols);
        
        Assert.Equal(clientInterface, service, symbols.Comparer);

        helper.AssertRequestMethod(clientMethods, symbols.Request1);
        helper.AssertRequestMethod(clientMethods, symbols.Request2);
            
        helper.AssertRequestMethod(clientInterfaceMethods, symbols.Request1);
        helper.AssertRequestMethod(clientInterfaceMethods, symbols.Request2);
    }

    private void AssertController(Compilation compilation, TestSymbols symbols)
    {
        var controllers = compilation.GetSymbolsWithName("GroupController", SymbolFilter.Type).OfType<INamedTypeSymbol>();
        var controller = Assert.Single(controllers);
        var actions = controller.GetMembers().OfType<IMethodSymbol>().ToList();
        var helper = new AssertSymbolHelper(symbols);

        Assert.Single(controller.GetAttributes().Where(t => symbols.Comparer.Equals(t.AttributeClass, symbols.ApiControllerAttribute)));
        
        AssertControllerMethod(helper.AssertRequestMethod(actions, symbols.Request1), "api/route1");
        AssertControllerMethod(helper.AssertRequestMethod(actions, symbols.Request2), "api/route2");
        
        return;

        void AssertControllerMethod(IMethodSymbol m, string route)
        {
            var attributes = m.GetAttributes();
            var modelAttributes = m.Parameters[0].GetAttributes();
            
            var routeAttribute = Assert.Single(attributes.Where(t => symbols.Comparer.Equals(t.AttributeClass, symbols.RouteAttribute)));
            var post = Assert.Single(attributes.Where(t => symbols.Comparer.Equals(t.AttributeClass, symbols.HttpPostAttribute)));
            
            Assert.Equal(route, routeAttribute.ConstructorArguments.First().Value);
            Assert.Single(modelAttributes.Where(t => symbols.Comparer.Equals(t.AttributeClass, symbols.FromBodyAttribute)));
        }
    }

    private void AssertService(Compilation compilation, TestSymbols symbols)
    {
        var controllerServices = compilation.GetSymbolsWithName("IGroupService", SymbolFilter.Type).OfType<INamedTypeSymbol>();
        var controllerService = Assert.Single(controllerServices);
        var methods = controllerService.GetMembers().OfType<IMethodSymbol>().ToList();
        var helper = new AssertSymbolHelper(symbols);

        helper.AssertRequestMethod(methods, symbols.Request1);
        helper.AssertRequestMethod(methods, symbols.Request2);
    }

    private void AssertRequestHandler(Compilation compilation, TestSymbols symbols)
    {
        var requestHandlers = compilation.GetSymbolsWithName("IGeneratedApiRequestHandler").OfType<INamedTypeSymbol>();
        var requestHandler = Assert.Single(requestHandlers);
        var baseRequestHandler = Assert.Single(requestHandler.Interfaces);
        
        Assert.Equal(symbols.IRequestHandler, baseRequestHandler, symbols.Comparer);
    }
    
    class TestSymbols(Compilation compilation) : KnownSymbols(compilation)
    {
        public readonly SymbolEqualityComparer Comparer = SymbolEqualityComparer.Default;
        public readonly INamedTypeSymbol Request1 = compilation.GetTypeByMetadataName("Templates.Request1")!;
        public readonly INamedTypeSymbol Request2 = compilation.GetTypeByMetadataName("Templates.Request2")!;
        public readonly INamedTypeSymbol Request2Response = compilation.GetTypeByMetadataName("Templates.Request2Response")!;
    }
}
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
        var result = CompilationFactory.CreateAndRunGenerators<ApiGenerator>(compilation, out var output);
        var symbols = new TestSymbols(output);
        
        // assert 
        Assert.Empty(result.Diagnostics);
        
        AssertService(output, symbols);
        AssertController(output, symbols);
        AssertRequestHandler(output, symbols);
        AssertClient(output, symbols);
        AssertClientService(output, symbols);
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
        
        Assert.Equal(clientInterface, service, symbols.Comparer);

        AssertRequestMethod(clientMethods, symbols.Request1, symbols);
        AssertRequestMethod(clientMethods, symbols.Request2, symbols);
            
        AssertRequestMethod(clientInterfaceMethods, symbols.Request1, symbols);
        AssertRequestMethod(clientInterfaceMethods, symbols.Request2, symbols);
    }

    private void AssertController(Compilation compilation, TestSymbols symbols)
    {
        var controllers = compilation.GetSymbolsWithName("GroupController", SymbolFilter.Type).OfType<INamedTypeSymbol>();
        var controller = Assert.Single(controllers);
        var actions = controller.GetMembers().OfType<IMethodSymbol>().ToList();

        Assert.Single(controller.GetAttributes().Where(t => symbols.Comparer.Equals(t.AttributeClass, symbols.ApiControllerAttribute)));
        
        AssertControllerMethod(AssertRequestMethod(actions, symbols.Request1, symbols), "api/route1");
        AssertControllerMethod(AssertRequestMethod(actions, symbols.Request2, symbols), "api/route2");
        
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

        AssertRequestMethod(methods, symbols.Request1, symbols);
        AssertRequestMethod(methods, symbols.Request2, symbols);
    }

    private void AssertRequestHandler(Compilation compilation, TestSymbols symbols)
    {
        var requestHandlers = compilation.GetSymbolsWithName("IGeneratedApiRequestHandler").OfType<INamedTypeSymbol>();
        var requestHandler = Assert.Single(requestHandlers);
        var baseRequestHandler = Assert.Single(requestHandler.Interfaces);
        
        Assert.Equal(symbols.IRequestHandler, baseRequestHandler, symbols.Comparer);
    }

    private IMethodSymbol AssertRequestMethod(List<IMethodSymbol> methods, INamedTypeSymbol request, TestSymbols symbols)
    {
        var method = Assert.Single(methods.Where(t => t.Name == request.Name));
        var result = symbols.Task;
        
        if (request.BaseType is { IsGenericType: true } baseType)
        {
            result = symbols.TaskGeneric.Construct(baseType.TypeArguments.Single());
        }
        
        Assert.Equal(2, method.Parameters.Length);
        Assert.Equal(method.Parameters[0].Type, request, symbols.Comparer);
        Assert.Equal(method.Parameters[1].Type, symbols.CancellationToken, symbols.Comparer);
        Assert.Equal(method.ReturnType, result, symbols.Comparer);

        return method;
    }
    
    class TestSymbols(Compilation compilation) : KnownSymbols(compilation)
    {
        public readonly SymbolEqualityComparer Comparer = SymbolEqualityComparer.Default;
        public readonly INamedTypeSymbol Request1 = compilation.GetTypeByMetadataName("Templates.Request1")!;
        public readonly INamedTypeSymbol Request2 = compilation.GetTypeByMetadataName("Templates.Request2")!;
        public readonly INamedTypeSymbol Request2Response = compilation.GetTypeByMetadataName("Templates.Request2Response")!;
    }
}
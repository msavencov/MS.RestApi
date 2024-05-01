using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MS.RestApi.SourceGenerator.Extensions;
using MS.RestApi.SourceGenerator.Generators;
using MS.RestApi.SourceGenerator.Tests.Helpers;

namespace MS.RestApi.SourceGenerator.Tests;

public class AttachmentTests
{
    [Fact]
    public void Default()
    {
        // arrange
        var compilation = CreateCompilation();
        var symbols = new TestSymbols(compilation);
        
        // act
        var result = CompilationFactory.CreateAndRunGenerators<ApiGenerator>(compilation);
        
        // assert 
        
        Assert.Empty(result.GetDiagnostics());
        
        AssertControllerMethod(result, symbols);
        AssertClientMethod(result, symbols);
    }

    private static void AssertControllerMethod(Compilation result, TestSymbols symbols)
    {
        var controllers = result.GetSymbolsWithName("TestController").OfType<INamedTypeSymbol>();
        var controller = Assert.Single(controllers);
        var controllerMethods = controller.GetMembers().OfType<IMethodSymbol>().ToList();
        var action = Assert.Single(controllerMethods.Where(t=>t.Name == "PostAttachmentRequest"));
        var attributes = action.GetAttributes().Where(t => symbols.Comparer.Equals(t.AttributeClass, symbols.BindFormFileAttribute))
                               .Select(t => (model: t.ConstructorArguments[0].Value, property: t.ConstructorArguments[1].Value))
                               .ToList();
        Assert.Single(attributes.Where(t => "model".Equals(t.model) && "Attachment".Equals(t.property)));
        Assert.Single(attributes.Where(t => "model".Equals(t.model) && "Attachments".Equals(t.property)));

    }

    private static void AssertClientMethod(Compilation result, TestSymbols symbols)
    {
        var clients = result.GetSymbolsWithName("TestApiClient").OfType<INamedTypeSymbol>();
        var client = Assert.Single(clients);
        var methods = client.GetMembers().OfType<IMethodSymbol>().ToList();
        var method = Assert.Single(methods.Where(t => t.Name == "AttachmentRequest"));
        var methodDeclaration = (MethodDeclarationSyntax)Assert.Single(method.DeclaringSyntaxReferences).GetSyntax();
        
        Assert.NotNull(methodDeclaration.Body);
        
        var statements = methodDeclaration.Body.Statements.OfType<ExpressionStatementSyntax>().ToList();
        
        Assert.Equal("attachments.Add(\"Attachment\", model.Attachment);", statements.First().ToString());
        Assert.Equal("attachments.Add(\"Attachments\", model.Attachments);", statements.Skip(1).First().ToString());
    }
    
    private static Compilation CreateCompilation()
    {
        var assembly = typeof(RouteParamTests).Assembly;
        var options = assembly.ReadEmbeddedResource("Attachment/Options.cs");
        var requests = new[] { "Attachment/Request.cs" }.Select(assembly.ReadEmbeddedResource);
        
        var contract = CompilationFactory.CreateContractAssembly(requests);
        var compilation = CompilationFactory.CreateCompilation(contract, options);

        return compilation;
    }
    
    private class TestSymbols(Compilation compilation) : KnownSymbols(compilation)
    {
        public readonly SymbolEqualityComparer Comparer = SymbolEqualityComparer.Default;
    }

}
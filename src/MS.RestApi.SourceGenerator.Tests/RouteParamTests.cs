using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
        
        Assert.Empty(result.GetDiagnostics());

        var controllers = result.GetSymbolsWithName("TestController").OfType<INamedTypeSymbol>();
        var controller = Assert.Single(controllers);
        var controllerMethods = controller.GetMembers().OfType<IMethodSymbol>().ToList();
        
        AssertControllerActions(controllerMethods);
        
        var clients = result.GetSymbolsWithName("TestApiClient").OfType<INamedTypeSymbol>();
        var client = Assert.Single(clients);
        var clientMethods = client.GetMembers().OfType<IMethodSymbol>().ToList();
        
        AssertClientActions(clientMethods);
    }

    private void AssertClientActions(List<IMethodSymbol> actions)
    {
        var recordRequest = Assert.Single(actions.Where(t => t.Name == "RecordRequest"));
        var recordRequestDeclaration = (MethodDeclarationSyntax)recordRequest.DeclaringSyntaxReferences.First().GetSyntax();
        
        Assert.NotNull(recordRequestDeclaration.Body);
        
        var recordRequestStatements = recordRequestDeclaration.Body.Statements.OfType<ExpressionStatementSyntax>().ToList();
        
        Assert.Equal("resource = resource.Replace(\"{Id}\", model.Id is {} ? model.Id.ToString", recordRequestStatements.First().ToString().Substring(0, 70));
        Assert.Equal("resource = resource.Replace(\"{Other}\", model.Other is {} ? model.Other.ToString", recordRequestStatements.Skip(1).First().ToString().Substring(0, 79));
    }
    private void AssertControllerActions(List<IMethodSymbol> actions)
    {
        var recordRequest = Assert.Single(actions.Where(t => t.Name == "PostRecordRequest"));
        var recordRequestDeclaration = (MethodDeclarationSyntax)recordRequest.DeclaringSyntaxReferences.First().GetSyntax();
        
        Assert.NotNull(recordRequestDeclaration.Body);
        
        var recordRequestStatements = recordRequestDeclaration.Body.Statements.OfType<ExpressionStatementSyntax>().ToList();
        
        Assert.Equal(2, recordRequestStatements.Count);
        
        Assert.Equal("model = model with { Id = Id };", recordRequestStatements.First().ToString());
        Assert.Equal("model = model with { Other = Other };", recordRequestStatements.Skip(1).First().ToString());
        
        var classRequest = Assert.Single(actions.Where(t=>t.Name == "PostClassRequest"));
        var classRequestDeclaration = (MethodDeclarationSyntax)classRequest.DeclaringSyntaxReferences.First().GetSyntax();

        Assert.NotNull(classRequestDeclaration.Body);
        
        var classRequestStatements = classRequestDeclaration.Body.Statements.OfType<ExpressionStatementSyntax>().ToList();
        
        Assert.Equal(2, classRequestStatements.Count);
        
        Assert.Equal("model.Id = Id;", classRequestStatements.First().ToString());
        Assert.Equal("model.Other = Other;", classRequestStatements.Skip(1).First().ToString());

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
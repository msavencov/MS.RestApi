using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.DependencyInjection;
using MS.RestApi.Abstractions;
using MS.RestApi.Client;
using MS.RestApi.Server;

namespace MS.RestApi.SourceGenerator.Tests.Extensions;

public static class CompilationFactory
{
    private static Assembly[] DefaultReferences()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        
        return
        [
            typeof(object).Assembly,
            assemblies.First(t => t.GetName().Name == "netstandard"),
            assemblies.First(t => t.GetName().Name == "System.Runtime"),
        ];
    }

    public static MetadataReference CreateContractAssembly(IEnumerable<string> source)
    {
        var syntaxTrees = source.Select(t => CSharpSyntaxTree.ParseText(t));
        var dependencies = new[] { typeof(IApiService) }.Select(t=>t.Assembly);
        var references = DefaultReferences().Union(dependencies).Select(t => MetadataReference.CreateFromFile(t.Location));
        var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release);
        var compilation = CSharpCompilation.Create("contract", syntaxTrees, references, options);
        var assemblyStream = new MemoryStream();

        if (compilation.Emit(assemblyStream).Diagnostics is { Length: > 0 } diagnostics && diagnostics.Any(t=>t.Severity == DiagnosticSeverity.Error))
        {
            throw new Exception(string.Join(Environment.NewLine, diagnostics.Select(t => t.GetMessage())));
        }
        
        assemblyStream.Seek(0, SeekOrigin.Begin);
        assemblyStream.Position = 0;
        
        return MetadataReference.CreateFromStream(assemblyStream);
    }

    public static Compilation CreateCompilation(MetadataReference contract, params string[] source)
    {
        var dependencies = new[]
        {
            typeof(WithServicesTests), typeof(Request), typeof(DependencyInjectionExtensions), typeof(IRequestHandler),
            typeof(Binder),typeof(ControllerBase), typeof(IServiceCollection), typeof(IMediator)
        }.Select(t => t.Assembly);
        var references = DefaultReferences().Union(dependencies).Select(t => MetadataReference.CreateFromFile(t.Location)).Union([contract]);
        var syntaxTrees = source.Select(t=> CSharpSyntaxTree.ParseText(t));
        var options = new CSharpCompilationOptions(OutputKind.NetModule);
        var compilation = CSharpCompilation.Create("compilation", syntaxTrees, references, options);

        if (compilation.GetDiagnostics() is { Length: > 0 } diagnostics && diagnostics.Any(t => t.Severity == DiagnosticSeverity.Error))
        {
            throw new Exception(string.Join(Environment.NewLine, diagnostics.Select(t => t.GetMessage())));
        }
        
        return compilation;
    }

    public static GeneratorDriverRunResult CreateAndRunGenerators<TGenerator>(Compilation compilation, out Compilation output) where TGenerator : IIncrementalGenerator, new()
    {
        var generator = new TGenerator();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        return driver.RunGeneratorsAndUpdateCompilation(compilation, out output, out _).GetRunResult();
    }
}
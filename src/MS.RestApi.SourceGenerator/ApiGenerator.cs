using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.CodeAnalysis;
using MS.RestApi.SourceGenerator.Descriptors;
using MS.RestApi.SourceGenerator.Extensions;
using MS.RestApi.SourceGenerator.Generators;

namespace MS.RestApi.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public class ApiGenerator : IIncrementalGenerator
{
    static ApiGenerator()
    {
        //while (System.Diagnostics.Debugger.IsAttached == false) System.Threading.Thread.Sleep(1_000);
    }

    public static readonly Assembly Assembly = typeof(ApiGenerator).Assembly;

    public void Initialize(IncrementalGeneratorInitializationContext generatorContext)
    {
        var optionsProvider = generatorContext.CompilationProvider.Select(GetGeneratorContext);
        
        generatorContext.RegisterSourceOutput(optionsProvider, (productionContext, descriptors) =>
        {
            foreach (var descriptor in descriptors)
            {
                foreach (var sourceCode in ApiGenPipeline.Run(descriptor, productionContext.ReportDiagnostic))
                {
                    productionContext.AddSource(sourceCode.Name, sourceCode.Source);
                }
            }
        });
        generatorContext.RegisterPostInitializationOutput(initializationContext =>
        {
            initializationContext.AddSource("MS.RestApi.GenerateServices.g.cs", Assembly.ReadEmbeddedResource("GenerateServices.cs"));
            initializationContext.AddSource("MS.RestApi.GenerateControllers.g.cs", Assembly.ReadEmbeddedResource("GenerateControllers.cs"));
            initializationContext.AddSource("MS.RestApi.ApiGenOptionsAttribute.g.cs", Assembly.ReadEmbeddedResource("ApiGenOptionsAttribute.cs"));
        });
    }

    private static IEnumerable<ApiGenContext> GetGeneratorContext(Compilation compilation, CancellationToken token)
    {
        var symbols = new KnownSymbols(compilation);
        var attributes = from t in compilation.Assembly.GetAttributes()
                         where SymbolEqualityComparer.Default.Equals(t.AttributeClass, symbols.ApiGenOptionsAttribute)
                         select t;

        foreach (var attribute in attributes)
        {
            yield return new ApiGenContext
            {
                Compilation = compilation,
                Options = new ApiGenOptions(attribute),
                Symbols = symbols,
                CancellationToken = token
            };
        }
    }
}
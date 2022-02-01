using System;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using MS.RestApi.Generators.Extensions;
using MS.RestApi.Generators.Utils;

namespace MS.RestApi.Generators
{
    [Generator]
    internal class ApiGenGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            // context.RegisterForPostInitialization(c =>
            // {
            //     c.AddSource("ApiGenOptionsAttribute.cs", typeof(ApiGenGenerator).Assembly.ReadCodeResource("ApiGenOptionsAttribute"));
            // });
        }

        public void Execute(GeneratorExecutionContext context)
        {
            try
            {
                var apiGenContext = ApiGenContext.Create(context);
                var pipe = new ApiGenGeneratorPipe(apiGenContext);
                
                pipe.Run();
            }
            catch (Exception e)
            {
                context.ReportDiagnosticError(e);
                throw;
            }
        }
    }
}
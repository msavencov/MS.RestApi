using System;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using MS.RestApi.SourceGenerator.Extensions;
using MS.RestApi.SourceGenerator.Utils;

namespace MS.RestApi.SourceGenerator
{
    [Generator]
    internal class ApiGenGenerator : ISourceGenerator
    {
        static ApiGenGenerator()
        {
            //System.Diagnostics.Debugger.Launch();
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

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForPostInitialization(initializationContext =>
            {
                var apiGenConfigAttribute = Assembly.GetExecutingAssembly().ReadEmbeddedResource("ApiGenConfigAttribute.tpl");
                var apiGenConfigAttributeName = $"MS.RestApi.SourceGenerator.ApiGenConfigAttribute.cs";
                
                initializationContext.AddSource(apiGenConfigAttributeName, apiGenConfigAttribute);
            });
        }
    }
}
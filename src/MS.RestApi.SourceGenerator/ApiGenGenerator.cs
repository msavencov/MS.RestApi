using System;
using Microsoft.CodeAnalysis;
using MS.RestApi.SourceGenerator.Extensions;
using MS.RestApi.SourceGenerator.Utils;

namespace MS.RestApi.SourceGenerator
{
    [Generator]
    internal class ApiGenGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            
        }

        public void Execute(GeneratorExecutionContext context)
        {
            //System.Threading.SpinWait.SpinUntil(() => System.Diagnostics.Debugger.IsAttached);
            
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
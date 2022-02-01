using MS.RestApi.Generators.Pipe;
using MS.RestApi.Generators.Utils;

namespace MS.RestApi.Generators.Common
{
    internal class AddSourceCode : IMiddleware<ApiGenContext>
    {
        public void Execute(ApiGenContext context)
        {
            foreach (var sourceCode in context.SourceCode)
            {
                context.Context.AddSource(sourceCode.Name, sourceCode.Source);
            }
        }
    }
}
using System;
using MS.RestApi.SourceGenerator.Pipe;
using MS.RestApi.SourceGenerator.Utils;

namespace MS.RestApi.SourceGenerator.Common;

internal class AddSourceCode : IMiddleware<ApiGenContext>
{
    public void Execute(ApiGenContext context)
    {
        foreach (var sourceCode in context.SourceCode)
        {
            context.Context.AddSource(sourceCode.Name, sourceCode.Source.Insert(0, "#pragma warning disable CS1591" + Environment.NewLine));
        }
    }
}
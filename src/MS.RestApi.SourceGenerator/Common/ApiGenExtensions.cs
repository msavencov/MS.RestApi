using Microsoft.CodeAnalysis;
using MS.RestApi.SourceGenerator.Extensions;
using MS.RestApi.SourceGenerator.Utils;

namespace MS.RestApi.SourceGenerator.Common;

internal static class ApiGenExtensions
{
    public static string GetResponseTypeName(this ApiGenRequest request, ApiGenContext context)
    {
        var result = request.Response.FullName();
        var comparer = SymbolEqualityComparer.Default;
        var taskSymbol = context.KnownSymbols.Task;

        if (comparer.Equals(request.Response, taskSymbol))
        {
            return taskSymbol.FullName();
        }

        return $"{taskSymbol.FullName()}<{result}>";
    }
}
using Microsoft.CodeAnalysis;
using MS.RestApi.Generators.Extensions;
using MS.RestApi.Generators.Utils;

namespace MS.RestApi.Generators.Common
{
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
}
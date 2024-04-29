using System.Collections.Generic;

namespace MS.RestApi.SourceGenerator.Helpers;

internal static class StringExtensions
{
    public static string Join(this IEnumerable<string> source, string delimiter = "") => string.Join(delimiter, source);
}
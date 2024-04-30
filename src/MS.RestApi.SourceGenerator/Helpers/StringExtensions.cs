using System;
using System.Collections.Generic;

namespace MS.RestApi.SourceGenerator.Helpers;

internal static class StringExtensions
{
    public static string Join(this IEnumerable<string> source, string delimiter = "") => string.Join(delimiter, source);
    public static string Quote(this string value, QuoteType type = QuoteType.Double)
    {
        return type switch
        {
            QuoteType.Single => $"'{value}'",
            QuoteType.Double => $"\"{value}\"",
            QuoteType.Braces => $"{{{value}}}",
            QuoteType.Parentheses => $"({value})",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
    
}
internal enum QuoteType { Single, Double, Braces, Parentheses}
using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace MS.RestApi.SourceGenerator.Extensions
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<TItem> Traverse<TItem>(this TItem input, Func<TItem, TItem> next) where TItem: ITypeSymbol
        {
            for (var item = next(input); item is { }; item = next(item))
            {
                yield return item;
            }
        }
        
        public static string Join(this IEnumerable<string> source, string delimiter)
        {
            return string.Join(delimiter, source);
        }
    }
}
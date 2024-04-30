using System.Collections.Generic;
using System.Linq;

namespace MS.RestApi.SourceGenerator.Helpers;

internal static class EnumerableExtensions
{
    public static IEnumerable<(TKey, TValue)> AsTuple<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source) => source.Select(t => (t.Key, t.Value));
}
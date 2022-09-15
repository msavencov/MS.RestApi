using System.Collections.Generic;

namespace MS.RestApi.Server.Extensions;

internal static class EnumerableExtensions
{
    public static string Join(this IEnumerable<string> input, string delimiter)
    {
        return string.Join(delimiter, input);
    }
}
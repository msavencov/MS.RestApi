using System;
using System.Collections.Generic;

namespace MS.RestApi.Client.Extensions;

public static class StringExtensions
{
    public static string Truncate(this string value, int length) => value is { } && value.Length > length ? value.Substring(0, length) : value;
    public static string Join(this IEnumerable<string> value, string delimiter) => string.Join(delimiter, value);
}
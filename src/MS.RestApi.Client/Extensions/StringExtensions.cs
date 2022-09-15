namespace MS.RestApi.Client.Extensions;

public static class StringExtensions
{
    public static string Truncate(this string value, int length) => value is { } && value.Length > length ? value.Substring(0, length) : value;
}
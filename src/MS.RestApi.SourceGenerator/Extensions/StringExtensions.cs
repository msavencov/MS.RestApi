using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace MS.RestApi.Generators.Extensions
{
    internal static class StringExtensions
    {
        public static TValue[] Parse<TValue>(this string[] value)
        {
            return value.Select(t => t.Trim().Parse<TValue>()).ToArray();
        }

        public static TValue Parse<TValue>(this object value)
        {
            if (value == null)
            {
                return default;
            }

            if (value is TValue v)
            {
                return v;
            }

            return (TValue)TypeDescriptor.GetConverter(typeof(TValue)).ConvertFrom(value);
        }

        public static TValue Parse<TValue>(this string value)
        {
            if (value == null)
            {
                return default;
            }

            if (value is TValue v)
            {
                return v;
            }

            return (TValue)TypeDescriptor.GetConverter(typeof(TValue)).ConvertFromInvariantString(value);
        }

        public static string[] Split(this string value, string delimiter)
        {
            return value?.Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
        }

        public static string Repeat(this string input, int count)
        {
            return string.Join("", Enumerable.Repeat(input, count));
        }
        
        public static string Quote(this string input, QuoteType type = QuoteType.Double)
        {
            var q = "\"";

            if (type == QuoteType.Single)
            {
                q = "'";
            }
            return $"{q}{input}{q}";
        }

        public static StringBuilder AppendLineIndented(this StringBuilder builder, int level, string line)
        {
            return builder.AppendLine("\t".Repeat(level) + line);
        }

        public static string ReadToEnd(this Stream stream)
        {
            using (var sr = new StreamReader(stream))
            {
                return sr.ReadToEnd();
            }
        }
    }

    internal enum QuoteType { Single, Double}
}
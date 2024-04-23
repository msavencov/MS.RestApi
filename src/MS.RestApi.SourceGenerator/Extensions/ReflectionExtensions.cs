﻿using System.IO;
using System.Linq;
using System.Reflection;
using MS.RestApi.SourceGenerator.Exceptions;

namespace MS.RestApi.SourceGenerator.Extensions;

internal static class ReflectionExtensions
{
    public static string ReadEmbeddedResource(this Assembly assembly, string resourceNameEndsWith)
    {
        var name = assembly.GetManifestResourceNames().Single(t => t.EndsWith(resourceNameEndsWith));
        var stream = assembly.GetManifestResourceStream(name);

        if (stream is {Length: > 0})
        {
            using (var sr = new StreamReader(stream))
            {
                return sr.ReadToEnd();
            }
        }

        throw new ApiGenException(1, $"The resource {resourceNameEndsWith} not found.");
    }
}
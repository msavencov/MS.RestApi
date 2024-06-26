﻿using System.Reflection;

namespace MS.RestApi.SourceGenerator.Extensions;

internal static class ReflectionExtensions
{
    public static string ReadEmbeddedResource(this Assembly assembly, string resourceNameEndsWith)
    {
        resourceNameEndsWith = resourceNameEndsWith.Replace('\\', '.').Replace('/', '.');
        
        var resourceName = assembly.GetManifestResourceNames().Single(t => t.EndsWith(resourceNameEndsWith));
        var stream = assembly.GetManifestResourceStream(resourceName);

        if (stream is {Length: > 0})
        {
            using var sr = new StreamReader(stream);
            return sr.ReadToEnd();
        }

        throw new Exception($"The resource with name ends with '{resourceNameEndsWith}' not found.");
    }
}
using System;

namespace MS.RestApi.Server.Extensions;

internal static class ReflectionExtensions
{
    public static string GetFullNameWithAssembly(this Type type)
    {
        return $"{type.FullName}, {type.Assembly.GetName().Name}";
    }
}
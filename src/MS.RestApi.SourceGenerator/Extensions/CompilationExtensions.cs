using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MS.RestApi.SourceGenerator.Extensions;

internal static class CompilationExtensions
{
    internal static string? GetBuildProperty(this AnalyzerConfigOptions options, string propertyName)
    {
        if (options.TryGetValue($"build_property.{propertyName}", out var value))
        {
            return value;
        }

        return default;
    }

    public static IEnumerable<INamedTypeSymbol> GetNamespaceMembersRecursive(this INamespaceSymbol namespaceSymbol)
    {
        foreach (var typeMember in namespaceSymbol.GetTypeMembers())
        {
            yield return typeMember;
        }

        foreach (var namespaceMember in namespaceSymbol.GetNamespaceMembers())
        {
            foreach (var typeSymbol in namespaceMember.GetNamespaceMembersRecursive())
            {
                yield return typeSymbol;
            }
        }
    }

    public static IEnumerable<INamedTypeSymbol> GetNamedTypeFromReferencedAssembly(this Compilation compilation, params string[] assemblyToScan)
    {
        foreach (var assemblyIdentity in compilation.SourceModule.ReferencedAssemblySymbols.Where(t => assemblyToScan.Contains(t.Name)))
        {
            foreach (var typeSymbol in assemblyIdentity.GlobalNamespace.GetNamespaceMembersRecursive())
            {
                yield return typeSymbol;
            }
        }
    }
}
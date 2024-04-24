using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace MS.RestApi.SourceGenerator.Extensions;

public static class CompilationExtensions
{
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
        return compilation.SourceModule.ReferencedAssemblySymbols
                          .Where(t => assemblyToScan.Contains(t.Name))
                          .SelectMany(t => t.GlobalNamespace.GetNamespaceMembersRecursive());
    }
}
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace MS.RestApi.SourceGenerator.Helpers;

public static class SymbolExtensions
{
    public static IEnumerable<INamedTypeSymbol> FindInterface(this INamedTypeSymbol symbol, INamedTypeSymbol target)
    {
        return symbol.Interfaces.Where(t => SymbolEqualityComparer.Default.Equals(t, target));
    }
    
    public static IEnumerable<INamedTypeSymbol> FindGenericInterface(this INamedTypeSymbol symbol, INamedTypeSymbol target)
    {
        if (target.IsUnboundGenericType == false)
        {
            target = target.ConstructUnboundGenericType();
        }
     
        var comparer = SymbolEqualityComparer.Default;
        
        foreach (var item in symbol.Interfaces.Where(t=>t.IsGenericType))
        {
            if (comparer.Equals(target, item.ConstructUnboundGenericType()))
            {
                yield return item;
            }
        }
    }
    
    public static IEnumerable<AttributeData> FindAttributes(this INamedTypeSymbol symbol, INamedTypeSymbol targetAttribute)
    {
        var comparer = SymbolEqualityComparer.Default;
        var attributes = symbol.GetAttributes();
        
        return attributes.Where(t => comparer.Equals(t.AttributeClass, targetAttribute));
    }
    public static IEnumerable<INamedTypeSymbol> GetNamedTypes(this IAssemblySymbol assembly)
    {
        var collector = new AssemblyTypes();
        
        assembly.GlobalNamespace.Accept(collector);

        return collector.CollectedTypes;
    }

    private class AssemblyTypes : SymbolVisitor
    {
        public readonly HashSet<INamedTypeSymbol> CollectedTypes = [];

        public override void VisitNamedType(INamedTypeSymbol symbol)
        {
            CollectedTypes.Add(symbol);

            foreach (var type in symbol.GetTypeMembers())
            {
                type.Accept(this); // Visit the nested type
            }
        }

        public override void VisitNamespace(INamespaceSymbol symbol)
        {
            foreach (var type in symbol.GetTypeMembers())
            {
                type.Accept(this); // Visit the type symbol
            }

            foreach (var @namespace in symbol.GetNamespaceMembers())
            {
                @namespace.Accept(this); // Visit the nested namespace
            }
        }

    }
}
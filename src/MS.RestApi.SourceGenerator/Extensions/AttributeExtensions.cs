using System.Linq;
using Microsoft.CodeAnalysis;

namespace MS.RestApi.Generators.Extensions
{
    internal static class AttributeExtensions
    {
        public static object GetValue(this AttributeData attributeData, string property)
        {
            if (attributeData.NamedArguments.Any(t => t.Key == property))
            {
                return attributeData.NamedArguments.Single(t => t.Key == property).Value.GetActualValue();
            }
            
            if (attributeData.AttributeConstructor is {} attributeConstructor)
            {
                var parameterSymbol = attributeConstructor.Parameters.FirstOrDefault(t => t.Name == property);

                if (parameterSymbol is { })
                {
                    var parameterIndex = attributeConstructor.Parameters.IndexOf(parameterSymbol);
                    var parameterArg = attributeData.ConstructorArguments[parameterIndex];

                    return parameterArg.GetActualValue() ?? parameterSymbol.ExplicitDefaultValue;
                }
            }
            
            return null;
        }

        private static object GetActualValue(this TypedConstant constant)
        {
            if (constant.Kind == TypedConstantKind.Array)
            {
                return constant.Values.Select(t => t.GetActualValue());
            }

            return constant.Value;
        }
        
        public static bool TryGetAttribute(this ISymbol symbol, ISymbol attributeSymbol, out AttributeData attribute)
        {
            attribute = default;
            
            foreach (var a in symbol.GetAttributes())
            {
                if (SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeSymbol))
                {
                    attribute = a;
                    return true;
                }
            }
            
            return false;
        }
    }
}
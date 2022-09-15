using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using MS.RestApi.SourceGenerator.Utils;

namespace MS.RestApi.SourceGenerator.Extensions;

internal static class AttributeDataExtensions
{
    public static ApiEndPoint ToApiEndPoint(this AttributeData attributeData)
    {
        var endPoint = new ApiEndPoint
        {
            Path = (string)attributeData.ConstructorArguments[0].GetConstantValue(),
            Service = (string)attributeData.ConstructorArguments[1].GetConstantValue()
        };

        return endPoint;
    }
    /*
    public static TAttribute MapToType<TAttribute>() where TAttribute : Attribute
    {
        TAttribute attribute = null;

        if (attributeData.AttributeConstructor is { } && attributeData.ConstructorArguments.Length > 0)
        {
            var args = new List<object>();

            foreach (var constructorArgument in attributeData.ConstructorArguments)
            {
                args.Add(constructorArgument.GetConstantValue());
            }

            attribute = (TAttribute)Activator.CreateInstance(typeof(TAttribute), args.ToArray());
        }
        else
        {
            attribute = (TAttribute)Activator.CreateInstance(typeof(TAttribute));
        }

        if (attribute == null)
        {
            return null;
        }

        foreach (var p in attributeData.NamedArguments)
        {
            typeof(TAttribute).GetField(p.Key).SetValue(attribute, p.Value.Value);
        }

        return attribute;
    }
    */
    public static object GetConstantValue(this TypedConstant constant)
    {
        if (constant.Kind == TypedConstantKind.Array)
        {
            //todo determine exact type of array. This one returns object[] always
            return constant.Values.Select(a => a.GetConstantValue()).ToArray();
        }

        if (constant.Kind == TypedConstantKind.Enum)
        {
            var typeName = constant.Type.ToString();
            var type = AppDomain.CurrentDomain
                                .GetAssemblies()
                                .Select(t => t.GetType(typeName))
                                .SkipWhile(t => t == null)
                                .First();

            return Enum.ToObject(type, constant.Value ?? 0);
        }

        return constant.Value;
    }
}
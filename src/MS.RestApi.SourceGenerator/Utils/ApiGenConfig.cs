using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using MS.RestApi.SourceGenerator.Extensions;

namespace MS.RestApi.SourceGenerator.Utils;

[AttributeUsage(AttributeTargets.Property)]
internal class ApiGenConfigAttribute : Attribute
{
    public string Name { get; }

    public ApiGenConfigAttribute(string name)
    {
        Name = name;
    }
}
internal class ApiGenConfig
{
    [ApiGenConfig("RootNamespace")] public string RootNamespace { get; private set; }
    [ApiGenConfig("AssemblyToScan")] public string[] AssemblyToScan { get; private set; }

    [ApiGenConfig("ApiName")] public string ApiName { get; private set; }
    [ApiGenConfig("ApiBaseRoute")] public string ApiBaseRoute { get; private set; }

    [ApiGenConfig("GenerateClient")] public bool GenerateClient { get; private set; }
    [ApiGenConfig("GenerateControllers")] public bool GenerateControllers { get; private set; }
    [ApiGenConfig("UseMediatorHandlers")] public bool UseMediatorHandlers { get; private set; }

    public string ClientRootNamespace { get; private set; }
    public string ClientServicesNamespace { get; private set; }
    public string ClientServicesImplNamespace { get; private set; }
    public string ClientExtensionsNamespace { get; private set; }

    public string ServerRootNamespace { get; private set; }
    public string ServerControllerNamespace { get; private set; }
    public string ServerServiceNamespace { get; private set; }
    public string ServerServiceImplNamespace { get; private set; }
    public string ServerExtensionsNamespace { get; private set; }

    public static ApiGenConfig Init(GeneratorExecutionContext context)
    {
        var config = new ApiGenConfig();
        var attributes = GetConfigAttributes(context);
            
        foreach (var property in typeof(ApiGenConfig).GetProperties())
        {
            if (attributes.TryGetValue(property.Name, out var value))
            {
                property.SetValue(config, value);
            }
        }

        config.ApiName ??= "GeneratedApi";
        config.ApiBaseRoute ??= "api";
        config.RootNamespace ??= context.GetBuildProperty("RootNamespace");
            
        config.ClientRootNamespace ??= $"{config.RootNamespace}.{config.ApiName}";
        config.ClientServicesNamespace ??= $"{config.ClientRootNamespace}.Services";
        config.ClientServicesImplNamespace ??= $"{config.ClientRootNamespace}.Client";
        config.ClientExtensionsNamespace ??= $"{config.ClientRootNamespace}.Extensions";

        config.ServerRootNamespace ??= $"{config.RootNamespace}.{config.ApiName}";
        config.ServerControllerNamespace ??= $"{config.ServerRootNamespace}.Controllers";
        config.ServerServiceNamespace ??= $"{config.ServerRootNamespace}.Services";
        config.ServerServiceImplNamespace ??= $"{config.ServerRootNamespace}.Services.Impl";
        config.ServerExtensionsNamespace ??= $"{config.ServerRootNamespace}.Extensions";

        return config;
    }

    private static Dictionary<string, object> GetConfigAttributes(GeneratorExecutionContext context)
    {
        var config = new Dictionary<string, object>();
        var symbol = context.Compilation.GetTypeByMetadataName("MS.RestApi.SourceGenerator.ApiGenConfigAttribute");
        var attributes = from t in context.Compilation.Assembly.GetAttributes()
                         where SymbolEqualityComparer.Default.Equals(t.AttributeClass, symbol)
                         select t;

        foreach (var attribute in attributes)
        {
            var keyArgument = attribute.ConstructorArguments.ElementAt(0);
            var valueArgument = attribute.ConstructorArguments.ElementAt(1);

            if (keyArgument.Value is not string {Length: > 0})
            {
                continue;
            }

            var key = keyArgument.Value.ToString();
            object value; 
                
            if (valueArgument.Kind == TypedConstantKind.Array)
            {
                value = valueArgument.Values.Select(t => (string) t.Value).ToArray();
            }
            else
            {
                value = valueArgument.Value;
            }
                
            config[key] = value;
        }

        return config;
    }
}
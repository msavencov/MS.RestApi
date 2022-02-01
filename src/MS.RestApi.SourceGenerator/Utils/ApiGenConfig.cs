using System;
using Microsoft.CodeAnalysis;
using MS.RestApi.Generators.Extensions;

namespace MS.RestApi.Generators.Utils
{
    internal class ApiGenConfig
    {
        public string RootNamespace { get; private set; }
        public string[] AssemblyToScan { get; private set; }
        
        public string ApiName { get; set; }
        public string ApiBaseRoute { get; set; }
        
        public bool GenerateClient { get; private set; }
        public string ClientInterfaceNamespace { get; set; }
        public string ClientImplementationNamespace { get; set; }
        public string ClientExtensionsNamespace { get; set; }
        
        public bool GenerateControllers { get; private set; }
        public string ControllerNamespace { get; set; }
        public string ControllerServiceNamespace { get; set; }
        
        public static ApiGenConfig Init(GeneratorExecutionContext context)
        {
            var config = new ApiGenConfig
            {
                GenerateControllers = context.GetApiGenBuildProperty("GenerateControllers").Parse<bool>(),
                GenerateClient = context.GetApiGenBuildProperty("GenerateClient").Parse<bool>(),
                AssemblyToScan = context.GetApiGenBuildProperty("AssemblyToScan").Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries).Parse<string>(),
                RootNamespace = context.GetBuildProperty("RootNamespace"),
                ControllerNamespace = context.GetApiGenBuildProperty("ControllerNamespace"),
                ApiName = context.GetApiGenBuildProperty("ApiName") ?? "Generated",
                ApiBaseRoute = context.GetApiGenBuildProperty("ApiBaseRoute") ?? "api",
            };
            
            if (string.IsNullOrEmpty(config.ControllerNamespace))
            {
                config.ControllerNamespace = $"{config.RootNamespace}.{config.ApiName}.Controllers";
            }

            if (string.IsNullOrEmpty(config.ControllerServiceNamespace))
            {
                config.ControllerServiceNamespace = $"{config.RootNamespace}.{config.ApiName}.Abstractions";
            }
            
            if (string.IsNullOrEmpty(config.ClientInterfaceNamespace))
            {
                config.ClientInterfaceNamespace = $"{config.RootNamespace}.{config.ApiName}.Abstractions";
            }

            if (string.IsNullOrEmpty(config.ClientImplementationNamespace))
            {
                config.ClientImplementationNamespace = $"{config.RootNamespace}.{config.ApiName}";
            }

            if (string.IsNullOrEmpty(config.ClientExtensionsNamespace))
            {
                config.ClientExtensionsNamespace = $"{config.ClientImplementationNamespace}.Extensions";
            }
            
            return config;
        }
    }
}
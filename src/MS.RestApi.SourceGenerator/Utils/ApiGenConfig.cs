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
                ApiName = context.GetApiGenBuildProperty("ApiName") ?? "Generated",
                RootNamespace = context.GetBuildProperty("RootNamespace"),
                ApiBaseRoute = context.GetApiGenBuildProperty("ApiBaseRoute") ?? "api",
            };
            config.RootNamespace += $".{config.ApiName}";
            
            if (string.IsNullOrEmpty(config.ControllerNamespace))
            {
                config.ControllerNamespace = $"{config.RootNamespace}.Controllers";
            }

            if (string.IsNullOrEmpty(config.ControllerServiceNamespace))
            {
                config.ControllerServiceNamespace = $"{config.RootNamespace}.Services.Abstractions";
            }
            
            if (string.IsNullOrEmpty(config.ClientInterfaceNamespace))
            {
                config.ClientInterfaceNamespace = $"{config.RootNamespace}.Client.Abstractions";
            }

            if (string.IsNullOrEmpty(config.ClientImplementationNamespace))
            {
                config.ClientImplementationNamespace = $"{config.RootNamespace}.{config.ApiName}.Client";
            }

            if (string.IsNullOrEmpty(config.ClientExtensionsNamespace))
            {
                config.ClientExtensionsNamespace = $"{config.ClientImplementationNamespace}.Extensions";
            }
            
            return config;
        }
    }
}
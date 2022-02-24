using System;
using Microsoft.CodeAnalysis;
using MS.RestApi.SourceGenerator.Extensions;

namespace MS.RestApi.SourceGenerator.Utils
{
    internal class ApiGenConfig
    {
        public string RootNamespace { get; private set; }
        public string[] AssemblyToScan { get; private set; }
        
        public string ApiName { get; set; }
        public string ApiBaseRoute { get; set; }
        
        public bool GenerateClient { get; private set; }
        public string ClientRootNamespace { get; private set; }
        public string ClientServicesNamespace { get; private set; }
        public string ClientServicesImplNamespace { get; private set; }
        public string ClientExtensionsNamespace { get; private set; }
        
        public bool GenerateControllers { get; private set; }
        public bool UseMediatorHandlers { get; private set; }
        public string ServerRootNamespace { get; private set; }
        public string ServerControllerNamespace { get; private set; }
        public string ServerServiceNamespace { get; private set; }
        public string ServerServiceImplNamespace { get; private set; }
        public string ServerExtensionsNamespace { get; private set; }

        public static ApiGenConfig Init(GeneratorExecutionContext context)
        {
            var config = new ApiGenConfig
            {
                GenerateControllers = context.GetApiGenBuildProperty("GenerateControllers").Parse<bool>(),
                UseMediatorHandlers = context.GetApiGenBuildProperty("UseMediatorHandlers").Parse<bool>(),
                GenerateClient = context.GetApiGenBuildProperty("GenerateClient").Parse<bool>(),
                AssemblyToScan = context.GetApiGenBuildProperty("AssemblyToScan").Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries).Parse<string>(),
                ApiName = context.GetApiGenBuildProperty("ApiName") ?? "GeneratedApi",
                RootNamespace = context.GetApiGenBuildProperty("RootNamespace") ?? context.GetBuildProperty("RootNamespace"),
                ApiBaseRoute = context.GetApiGenBuildProperty("ApiBaseRoute") ?? "api",
            };
            
            config.ClientRootNamespace = $"{config.RootNamespace}.{config.ApiName}";
            config.ClientServicesNamespace = $"{config.ClientRootNamespace}.Services";
            config.ClientServicesImplNamespace = $"{config.ClientRootNamespace}.Client";
            config.ClientExtensionsNamespace = $"{config.ClientRootNamespace}.Extensions";
            
            config.ServerRootNamespace = $"{config.RootNamespace}.{config.ApiName}";
            config.ServerControllerNamespace = $"{config.ServerRootNamespace}.Controllers";
            config.ServerServiceNamespace = $"{config.ServerRootNamespace}.Services";
            config.ServerServiceImplNamespace = $"{config.ServerRootNamespace}.ServicesImpl";
            config.ServerExtensionsNamespace = $"{config.ServerRootNamespace}.Extensions";
            
            return config;
        }
    }
}
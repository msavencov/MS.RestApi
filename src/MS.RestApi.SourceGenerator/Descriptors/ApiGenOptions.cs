using System;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;

namespace MS.RestApi.SourceGenerator.Descriptors;

public class ApiGenOptions
{
    [ApiGenConfig("ContractAssembly")] public string ContractAssembly { get; private set; } = null!;
    [ApiGenConfig("RootNamespace")] public string RootNamespace { get; private set; } = null!;

    [ApiGenConfig("ApiName")] public string ApiName { get; private set; } = null!;
    [ApiGenConfig("ApiBaseRoute")] public string ApiBaseRoute { get; private set; }
    
    [ApiGenConfig("GenerateClient")] public bool GenerateClient { get; private set; }
    [ApiGenConfig("GenerateControllers")] public bool GenerateControllers { get; private set; }
    [ApiGenConfig("GenerateServices")] public bool GenerateServices { get; private set; }
    [ApiGenConfig("UseMediatorHandlers")] public bool UseMediatorHandlers { get; private set; }
    
    public ClientConventions ClientConventions { get; private set; }
    public ServerConventions ServerConventions { get; private set; }

    public ApiGenOptions(AttributeData attribute)
    {
        var properties = from p in typeof(ApiGenOptions).GetProperties()
                         from a in p.GetCustomAttributes<ApiGenConfigAttribute>()
                         join o in attribute.NamedArguments on a.Name equals o.Key
                         select (Config: p, o.Value);

        foreach (var (property, constant) in properties)
        {
            var value = constant switch
            {
                { Kind: TypedConstantKind.Array } => constant.Values.Select(t => t.Value).ToArray(),
                _ => constant.Value,
            };
            property.SetValue(this, value);
        }

        ApiBaseRoute ??= "api";
        
        ClientConventions = new ClientConventions(this);
        ServerConventions = new ServerConventions(this);
    }
    
    public string GetRoute(string path)
    {
        if (string.IsNullOrEmpty(ApiBaseRoute))
        {
            return path.Trim('/');
        }
        
        return $"{ApiBaseRoute.Trim('/')}/{path.Trim('/')}";
    }
}

public class ClientConventions(ApiGenOptions options)
{
    public string RootNamespace => $"{options.RootNamespace}.{options.ApiName}";
    public string ServicesNamespace => $"{RootNamespace}.Services";
    public string ServicesImplNamespace => $"{RootNamespace}.Client";
    public string ExtensionsNamespace => $"{RootNamespace}.Extensions";

    public (string Name, string Namespace) GetApiService(string service) => ($"I{service}Api", ServicesNamespace);
    public (string Name, string Namespace) GetRequestHandler(INamedTypeSymbol requestHandler) => (requestHandler.Name.Insert(1, options.ApiName), ServicesNamespace);
    public (string Name, string Namespace) GetClientService(string service) => ($"{service}ApiClient", ServicesImplNamespace);
}

public class ServerConventions(ApiGenOptions options)
{
    public string RootNamespace => $"{options.RootNamespace}.{options.ApiName}";
    public string ControllerNamespace => $"{RootNamespace}.Controllers";
    public string ServiceNamespace => $"{RootNamespace}.Services";
    public string ServiceImplNamespace => $"{RootNamespace}.Services.Impl";
    public string ExtensionsNamespace => $"{RootNamespace}.Extensions";

    public (string Name, string Namespace) ControllerName(string service) => ($"{service}Controller", ControllerNamespace);
    public (string Name, string Namespace) ServiceInterface(string service) => ($"I{service}Service", ServiceNamespace);
}

file class ApiGenConfigAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}
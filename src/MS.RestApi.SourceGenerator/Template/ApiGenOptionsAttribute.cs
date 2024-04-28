namespace MS.RestApi;

[System.AttributeUsage(System.AttributeTargets.Assembly, AllowMultiple = true)]
internal class ApiGenOptionsAttribute : System.Attribute
{
    public required string ContractAssembly { get; set; }
    public string RootNamespace { get; set; } = "ApiGen";
    public string ApiName { get; set; } = "GeneratedApi";
    public string ApiBaseRoute { get; set; } = "api";
    public GenerateControllers GenerateControllers { get; set; } = GenerateControllers.None;
    public GenerateServices GenerateServices { get; set; } = GenerateServices.None;
    public bool GenerateClient { get; set; }
}

internal enum GenerateControllers { None, WithService, WithMediator }
internal enum GenerateServices { None, WithService, WithMediator }
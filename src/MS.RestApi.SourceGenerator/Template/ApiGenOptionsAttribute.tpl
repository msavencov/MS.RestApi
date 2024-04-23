namespace MS.RestApi
{
    [System.AttributeUsage(System.AttributeTargets.Assembly, AllowMultiple = true)]
    internal class ApiGenOptionsAttribute : System.Attribute
    {
        public required string ContractAssembly { get; set; }
        public bool GenerateControllers { get; set; }
        public bool UseMediatorHandlers { get; set; }
        public bool GenerateServices { get; set; }
        public bool GenerateClient { get; set; }
        public required string ApiName { get; set; }
        public required string RootNamespace { get; set; }
        public string ApiBaseRoute { get; set; }        
    }
}
using MS.RestApi;

[assembly: MS.RestApi.ApiGenOptionsAttribute(ContractAssembly = "contract", GenerateControllers = GenerateControllers.WithService, GenerateServices = GenerateServices.WithService, GenerateClient = true )]
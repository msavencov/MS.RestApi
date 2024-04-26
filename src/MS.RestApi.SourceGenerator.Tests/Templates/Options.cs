using MS.RestApi;

[assembly: MS.RestApi.ApiGenOptionsAttribute(ContractAssembly = "contract", GenerateControllers = GenerateControllers.WithMediator, GenerateServices = GenerateServices.WithMediator, GenerateClient = true )]
# MS.RestApi

MS.RestApi is a set of source generators that allow to generate client/server code based on shared contract assembly.

The primary concept of this generator is to have defined a shared library that defines API in code which is referenced by Client and Server. 

## CI Status
[![Build status](https://ci.appveyor.com/api/projects/status/yx56lwlibg15bjwv/branch/master?svg=true)](https://ci.appveyor.com/project/msavencov/ms-restapi/branch/master)

## NuGet feeds
- nuget https://www.nuget.org - TBD
- appveyor https://ci.appveyor.com/nuget/ms-restapi

## Quick Start

### Create shared project

```shell
dotnet new classlib -n contract
```
* Add reference to API Gen abstractions library

```shell
dotnet add contract package MS.RestApi
```

* Add reference to metadata annotations for supporting default ASP.NET Core validation filter. 
```shell
dotnet add server package System.ComponentModel.Annotations
```

* Add API operation endpoint definition
```c#
using System.ComponentModel.DataAnnotations;
using MS.RestApi.Abstractions;

namespace contract
{
    [EndPoint("account/signin/local", "Account")]
    public class Signin : Request<SigninResponse>
    {
        [Required, EmailAddress]
        public string Username { get; set; }
        
        [Required, MinLength(6), MaxLength(50)]
        public string Password { get; set; }
    }

    public class SigninResponse
    {
        public string AccessToken { get; set; }
    }
}
```

### Create server project

```shell
dotnet new web -n server 
```

* Add references to API Gen library
```shell
dotnet add server package MS.RestApi.Server
dotnet add server package MS.RestApi.SourceGenerator
```

* Add references to shared library 
```shell
dotnet add server reference contract
```

* Configure API Gen to generate controllers
```csharp
[assembly: MS.RestApi.SourceGenerator.ApiGenConfig("AssemblyToScan", new []{"contract"})]
[assembly: MS.RestApi.SourceGenerator.ApiGenConfig("GenerateControllers", true)]
```

* Register exception filter service in `ConfigureServices(IServiceCollection services)`
```csharp
services.AddControllers(options =>
{
    options.Filters.Add<ExceptionHandlerFilterAttribute>();
});
```

* Implement generated service interface
```csharp
internal class AccountService : IAccountService
{
    public Task<SigninResponse> SigninAsync(Signin model, CancellationToken ct = default)
    {
        return Task.FromResult(new SigninResponse
        {
            AccessToken = Guid.NewGuid().ToString()
        });
    }
}
```

* Register implemented service in `ConfigureServices(IServiceCollection services)`
```c#
services.AddScoped<IAccountService, AccountService>();
```

### Create client project 

```shell
dotnet new console --name client
```

* Add references to shared library
```shell
dotnet add client reference contract
```

* Add references to API Gen library
```shell
dotnet add client package MS.RestApi.Client
dotnet add client package MS.RestApi.SourceGenerator
```

* Configure API Gen to generate client
```csharp
[assembly: MS.RestApi.SourceGenerator.ApiGenConfig("GenerateClient", true)]
[assembly: MS.RestApi.SourceGenerator.ApiGenConfig("AssemblyToScan", new[] {"contract"})]
```

* Implement generated request handler interface 
```csharp
internal class DefaultRequestHandler : RequestHandlerBase, IGeneratedApiRequestHandler
{
    public DefaultRequestHandler(HttpClient client) : base(client)
    {
        
    }
}
```

* Register api client dependencies and configure generated services 
```csharp
private static IServiceProvider BuildServiceProvider(IServiceCollection services = default)
{
    services.AddHttpClient<IGeneratedApiRequestHandler, DefaultRequestHandler>()
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri("http://localhost:5000/api");
            });
    services.AddGeneratedApi(options =>
    {
        options.ServiceLifetime = ServiceLifetime.Transient;
    });
    
    return services.BuildServiceProvider();
}
```

* Usage
```csharp
var services = BuildServiceProvider();
var accountApi = services.GetRequiredService<IAccountApi>();
try
{
    var result = await accountApi.SigninAsync(new Signin(), CancellationToken.None);
}
catch (ApiRemoteErrorException errorException)
{
    // handle API error
}
```

### Generator configuration options

To use configure generator properties you should add the assembly attribute `ApiGenConfigAttribute`

The Minimal configuration for server projects:
```csharp
[assembly: MS.RestApi.SourceGenerator.ApiGenConfig("GenerateControllers", true)]
[assembly: MS.RestApi.SourceGenerator.ApiGenConfig("AssemblyToScan", new []{"contract"})]
```

The Minimal configuration for client projects:
```csharp
[assembly: MS.RestApi.SourceGenerator.ApiGenConfig("GenerateClient", true)]
[assembly: MS.RestApi.SourceGenerator.ApiGenConfig("AssemblyToScan", new[] {"contract"})]
```

Below is the list of configurable generator properties.

| Property                        | Type     | Default          | Description                                                                           |
|---------------------------------|----------|------------------|---------------------------------------------------------------------------------------|
| AssemblyToScan                  | string[] |                  | The comma delimited list of assembly names in witch generator should scan for request |
| GenerateControllers             | bool     | false            | Generate controllers for all requests defined in `AssemblyToScan` assemblies          |
| GenerateClient                  | bool     | false            | Generate client interfaces and their implementations                                  |
| ApiName                         | string   | GeneratedApi     | The namespace prefix for all generated types                                          |
| RootNamespace                   | string   | $(RootNamespace) | The root namespace for all generated code                                             |
| ApiBaseRoute                    | string   | api              | The base route for generated endpoints                                                |
| UseMediatorHandlers<sup>1</sup> | bool     | false            | By setting property to true controllers will use IMediator for dispatching requests   | 
 
Notice:
1. The MediatR nuget package `<PackageReference Include="MediatR" Version="9.0.0" />` must be referenced.
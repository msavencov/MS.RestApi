# MS.RestApi

NuGet feeds
- nuget https://www.nuget.org/packages/MS.RestApi/ - TBD
- appveyor https://ci.appveyor.com/nuget/ms-restapi-r9yfjma0f07n

MS.RestApi is a set of source generators that allow to generate client/server code based on shared contract assembly.

The primary concept of this generator is to have defined a shared library that defines API in code which is referenced by Client and Server. 

## CI Status
[![Build status](https://ci.appveyor.com/api/projects/status/yx56lwlibg15bjwv/branch/master?svg=true)](https://ci.appveyor.com/project/msavencov/ms-restapi/branch/master)

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
    [EndPoint(Method.Post, "account/signin/local", "Account")]
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

* configure source generator in `server/server.csproj`
```xml
  <ItemGroup>
    <CompilerVisibleProperty Include="ApiGenAssemblyToScan" /> 
    <CompilerVisibleProperty Include="ApiGenGenerateControllers" />
  </ItemGroup>

  <PropertyGroup>
    <ApiGenAssemblyToScan>contract</ApiGenAssemblyToScan>
    <!-- tell to generator where to scan for operations -->
    
    <ApiGenGenerateControllers>true</ApiGenGenerateControllers> 
    <!-- tell to generator that we want to generate controllers -->
  </PropertyGroup>
```

* Register exception filter service in `ConfigureServices(IServiceCollection services)`
```c#
services.AddControllers(options =>
{
    options.Filters.Add<ExceptionHandlerFilterAttribute>();
});
```

* Implement generated service interface
```c#
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

* configure source generator in `client/client.csproj`
```xml
  <ItemGroup>
    <CompilerVisibleProperty Include="ApiGenAssemblyToScan" /> 
    <CompilerVisibleProperty Include="ApiGenGenerateClient" />
  </ItemGroup>

  <PropertyGroup>
    <ApiGenAssemblyToScan>contract</ApiGenAssemblyToScan>
    <!-- tell to generator where to scan for operations -->
    
    <ApiGenGenerateClient>true</ApiGenGenerateClient> 
    <!-- tell to generator that we want to generate client implementation -->
  </PropertyGroup>
```

* Implement generated request handler interface 
```c#
internal class DefaultRequestHandler : RequestHandlerBase, IGeneratedRequestHandler
{
    public DefaultRequestHandler(HttpClient client) : base(client)
    {
        
    }
}
```

* Register api client dependencies and configure generated services 
```c#
private static IServiceProvider BuildServiceProvider(IServiceCollection services = default)
{
    services.AddHttpClient<IGeneratedRequestHandler, DefaultRequestHandler>()
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri("http://localhost:5269");
            });
    services.AddGeneratedApi(options =>
    {
        options.ServiceLifetime = ServiceLifetime.Transient;
    });
    
    return services.BuildServiceProvider();
}
```

* Usage
```c#
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

To use generator configuration properties you should define the property as visible for compiler.
```xml
  <ItemGroup>
    <CompilerVisibleProperty Include="ApiGenAssemblyToScan" />
  </ItemGroup>
```

Then assign the value 
```xml
  <PropertyGroup>
    <ApiGenAssemblyToScan>My.Assembly</ApiGenAssemblyToScan>
  </PropertyGroup>
```

Below is the list of generator configuration properties.

| Property                  | Type   | Default   | Description                                                                           |
|---------------------------|--------|-----------|---------------------------------------------------------------------------------------|
| ApiGenAssemblyToScan      | string |           | The comma delimited list of assembly names in witch generator should scan for request |
| ApiGenGenerateControllers | bool   | False     | Generate controllers for all requests defined in `ApiGenAssemblyToScan` assemblies    |
| ApiGenGenerateClient      | bool   | False     | Generate client interfaces and their implementations                                  |
| ApiGenApiName             | string | Generated | The namespace prefix for all generated types                                          |
| ApiGenApiBaseRoute        | string | api       | The base route for generated endpoints                                                |
| ApiGenUseMediatorHandlers | bool   | False     | By setting property to true controllers will use IMediator for dispatching requests   | 

Assume that the root namespaces of assembly is referencing source generator is `My.Company.Api`

The root namespace for all generated code consist of the root namespace of referencing assembly ends with `ApiGenApiName` property's value, witch results in `My.Company.Api.Generated`  
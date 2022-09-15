using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace MS.RestApi.SourceGenerator.Utils;

internal class ApiGenRequest
{
    public ApiEndPoint EndPoint { get; set; }
    public ISymbol Request { get; set; }
    public ISymbol Response { get; set; }

    public static string BuildClientName(string group)
    {
        return $"{group}ApiClient";
    }
        
    public static string BuildInterfaceName(string group)
    {
        return $"I{group}Api";
    }

    public string GetMethodName()
    {
        return $"{Request.Name}Async";
    }

    public string GetEndpointRoute(ApiGenConfig config)
    {
        var prefix = config.ApiBaseRoute.Trim('/');
        var route = EndPoint.Path.Trim('/');

        return $"{prefix}/{route}";
    }
}
    
internal class ApiEndPoint
{
    public string Service { get; set; }
    public string Path { get; set; }
}

internal class ApiGenService
{
    public string ServiceName { get; init; }
    public ApiGenRequest[] Operations { get; init; }
}
    
internal class ApiGenRequestCollection : HashSet<ApiGenRequest>
{
    private readonly Lazy<ApiGenService[]> _services;

    public ApiGenRequestCollection()
    {
        _services = new Lazy<ApiGenService[]>(ServicesFactory);
    }

    private ApiGenService[] ServicesFactory()
    {
        var services = from service in this
                       group service by service.EndPoint.Service into s
                       select new ApiGenService
                       {
                           ServiceName = s.Key,
                           Operations = s.ToArray()
                       };
        return services.ToArray();
    }

    public ApiGenService[] AsServices()
    {
        return _services.Value;
    }
}
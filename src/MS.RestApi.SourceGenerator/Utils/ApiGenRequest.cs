using Microsoft.CodeAnalysis;
using MS.RestApi.Abstractions;

namespace MS.RestApi.Generators.Utils
{
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
        public Method Method { get; set; }
        public string Service { get; set; }
        public string Path { get; set; }
    }
}
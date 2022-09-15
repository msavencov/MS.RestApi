using System.Linq;
using Microsoft.CodeAnalysis;
using MS.RestApi.SourceGenerator.Extensions;
using MS.RestApi.SourceGenerator.Pipe;
using MS.RestApi.SourceGenerator.Utils;

namespace MS.RestApi.SourceGenerator.Common;

internal class AddApiRequests : IMiddleware<ApiGenContext>
{
    public void Execute(ApiGenContext apiGenContext)
    {
        var context = apiGenContext.Context;
        var config = apiGenContext.Config;
        var symbols = apiGenContext.KnownSymbols;
            
        var compilation = context.Compilation;
        var symbolComparer = SymbolEqualityComparer.Default;
        var baseRequestSymbol = symbols.Request;
        var taskSymbol = symbols.Task;
        var assembliesToScan = config.AssemblyToScan;
            
        foreach (var requestTypeSymbol in compilation.GetNamedTypeFromReferencedAssembly(assembliesToScan))
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            if (requestTypeSymbol.TryGetAttribute(symbols.EndPointAttribute, out var endPointAttributeData) == false)
            {
                continue;
            }

            var apiEndPoint = endPointAttributeData.ToApiEndPoint();
            var apiRequest = new ApiGenRequest
            {
                Request = requestTypeSymbol,
                EndPoint = apiEndPoint,
            };

            var baseTypes = requestTypeSymbol.SelectRecursive(t => t.BaseType);

            foreach (var baseType in baseTypes)
            {
                if (baseType.IsGenericType && symbolComparer.Equals(baseType.BaseType, baseRequestSymbol))
                {
                    apiRequest.Response = baseType.TypeArguments.Single();
                }
                else if (symbolComparer.Equals(baseType, baseRequestSymbol))
                {
                    apiRequest.Response = taskSymbol;
                }

                if (apiRequest.Response is { } )
                { 
                    break;
                }
            }

            if (apiRequest.Response == null)
            {
                throw new ApiGenException(100, $"The request should extend the '{baseRequestSymbol.FullName()}'.");
            }

            apiGenContext.Requests.Add(apiRequest);
        }
    }
}
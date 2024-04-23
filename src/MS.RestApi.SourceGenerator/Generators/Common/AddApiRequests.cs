using System.Linq;
using Microsoft.CodeAnalysis;
using MS.RestApi.SourceGenerator.Descriptors;
using MS.RestApi.SourceGenerator.Extensions;
using MS.RestApi.SourceGenerator.Extensions.Pipe;

namespace MS.RestApi.SourceGenerator.Generators.Common;

internal class AddApiRequests : IMiddleware<ApiGenContext>
{
    public void Execute(ApiGenContext context)
    {
        var comparer = SymbolEqualityComparer.Default;
        var compilation = context.Compilation;
        var symbols = context.Symbols;
        
        foreach (var request in compilation.GetNamedTypeFromReferencedAssembly(context.Options.ContractAssembly))
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var conversion = compilation.ClassifyCommonConversion(request, symbols.Request);
            if (conversion is not { Exists: true, IsImplicit: true })
            {
                continue;
            }
            
            var response = default(INamedTypeSymbol?);

            for (var baseType = request.BaseType; baseType is { }; baseType = baseType.BaseType)
            {
                if (baseType.IsGenericType && comparer.Equals(baseType.BaseType, symbols.Request))
                {
                    response ??= (INamedTypeSymbol)baseType.TypeArguments.Single();
                    break;
                }
            }

            var attributes = request.GetAttributes();
            var endpoint = attributes.Where(t => comparer.Equals(t.AttributeClass, symbols.EndPointAttribute)).Select(MapEndpoint).First();

            context.AddRequest(endpoint.Service, new ApiRequestDescriptor
            {
                Service = endpoint.Service,
                Endpoint = endpoint.Endpoint,
                Request = request,
                Response = response,
            });
        }
    }

    private static (string Service, string Endpoint) MapEndpoint(AttributeData attribute)
    {
        var service = (string)attribute.ConstructorArguments[1].Value!;
        var endpoint = (string)attribute.ConstructorArguments[0].Value!;

        return (service, endpoint);
    }
}
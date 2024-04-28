using System.Linq;
using Microsoft.CodeAnalysis;
using MS.RestApi.SourceGenerator.Descriptors;
using MS.RestApi.SourceGenerator.Helpers;
using MS.RestApi.SourceGenerator.Helpers.Pipe;

namespace MS.RestApi.SourceGenerator.Generators.Common;

internal class AddApiRequests : IMiddleware<ApiGenContext>
{
    public void Execute(ApiGenContext context)
    {
        var compilation = context.Compilation;
        var symbols = context.Symbols;
        var assembly = compilation.SourceModule.ReferencedAssemblySymbols.Single(t => t.Name == context.Options.ContractAssembly);
        
        foreach (var request in assembly.GetNamedTypes())
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            
            var (valid, response) = GetResponse(request, symbols);
            
            if (valid == false)
            {
                continue;
            }
            
            foreach (var attribute in request.FindAttributes(symbols.EndPointAttribute))
            {
                var service = (string)attribute.ConstructorArguments[1].Value!;
                var endpoint = (string)attribute.ConstructorArguments[0].Value!;
                
                context.AddRequest(service, new ApiRequestDescriptor
                {
                    Service = service,
                    Endpoint = endpoint,
                    Request = request,
                    Response = response,
                });
            }
        }
    }

    private static (bool Valid, INamedTypeSymbol? Response) GetResponse(INamedTypeSymbol request, KnownSymbols symbols)
    {
        if (request.FindGenericInterface(symbols.IApiRequest1).SingleOrDefault() is { } ar)
        {
            return (true, (INamedTypeSymbol)ar.TypeArguments.Single());
        }
        
        if (request.FindInterface(symbols.IApiRequest).SingleOrDefault() is not null)
        {
            return (true, default);
        }
        
        if (request.FindGenericInterface(symbols.MediatorRequest1).SingleOrDefault() is { } @interface)
        {
            return (true, (INamedTypeSymbol?)@interface.TypeArguments.Single());
        }

        if (request.FindInterface(symbols.MediatorRequest).SingleOrDefault() is not null)
        {
            return (true, default);
        }

        return (false, default);
    }
}
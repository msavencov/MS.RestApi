using System.Linq;
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
        var helper = new RequestSymbolHelper(symbols);
        var assembly = compilation.SourceModule.ReferencedAssemblySymbols.Single(t => t.Name == context.Options.ContractAssembly);
        
        foreach (var request in assembly.GetNamedTypes())
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            
            var (valid, response) = helper.ValidateRequest(request);
            
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
}
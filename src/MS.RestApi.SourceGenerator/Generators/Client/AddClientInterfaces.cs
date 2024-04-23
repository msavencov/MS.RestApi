using System.Collections.Generic;
using System.Text;
using MS.RestApi.SourceGenerator.Descriptors;
using MS.RestApi.SourceGenerator.Extensions;
using MS.RestApi.SourceGenerator.Extensions.Pipe;

namespace MS.RestApi.SourceGenerator.Generators.Client;

internal class AddClientInterfaces : IMiddleware<ApiGenContext>
{
    public void Execute(ApiGenContext context)
    {
        AddClientHandlerInterface(context);
        AddClientServiceInterfaces(context);
    }

    private void AddClientServiceInterfaces(ApiGenContext context)
    {
        var config = context.Options;
        var symbols = context.KnownSymbols;
        var conventions = config.ClientConventions;
        
        var builder = new StringBuilder();
        var writer = new IndentedWriter(builder);

        var requestHandler = symbols.ClientRequestHandlerInterface.ToDisplayString();
        var apiRequestHandler = conventions.GetRequestHandler(symbols.ClientRequestHandlerInterface);
        
        writer.WriteLine($"namespace {apiRequestHandler.Namespace}");
        writer.WriteBlock(nsw =>
        {
            nsw.WriteLine($"public interface {apiRequestHandler.Name} : {requestHandler}");
            nsw.WriteBlock(_ => { });
        });
            
        context.Result.Add(new ApiGenSourceCode
        {
            Name = $"{apiRequestHandler.Namespace}.{apiRequestHandler.Name}.cs",
            Source = builder.ToString()
        });
    }

    private void AddClientHandlerInterface(ApiGenContext context)
    {
        foreach (var (serviceName, requests) in context.Services)
        {
            var sourceCode = new ApiGenSourceCode
            {
                Name = $"{context.Options.ClientConventions.ServicesNamespace}.{serviceName}.cs",
                Source = BuildInterfaceSourceCode(serviceName, requests, context),
            };
                
            context.Result.Add(sourceCode);
        }
    }
        
    private string BuildInterfaceSourceCode(string service, IEnumerable<ApiRequestDescriptor> actions, ApiGenContext context)
    {
        var builder = new StringBuilder();
        var writer = new IndentedWriter(builder, 0);
            
        var config = context.Options;
        var symbol = context.KnownSymbols;
        var conventions = config.ClientConventions;
        var serviceInterface = conventions.GetApiService(service);
        
        writer.WriteLine($"namespace {serviceInterface.Namespace}");
        writer.WriteBlock(ns =>
        {
            ns.WriteLine($"public interface {serviceInterface.Name} : {symbol.IApiService.ToDisplayString()}");
            ns.WriteBlock(ib =>
            {
                foreach (var request in actions)
                {
                    var ct = symbol.CancellationToken.ToDisplayString();
                    var model = request.Request.ToDisplayString();
                    var methodName = request.Request.Name;
                    var returnResult = request.GetResponseTypeName(symbol.Task);
                        
                    ib.WriteLine($"/// <inheritdoc cref=\"{model}\"/>");
                    ib.WriteLine($"{returnResult} {methodName}({model} model, {ct} ct = default);");
                }
            });
        });

        return builder.ToString();
    }
}
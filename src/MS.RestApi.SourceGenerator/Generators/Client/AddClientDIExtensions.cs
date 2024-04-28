using System.Linq;
using System.Text;
using MS.RestApi.SourceGenerator.Helpers;
using MS.RestApi.SourceGenerator.Helpers.Pipe;

namespace MS.RestApi.SourceGenerator.Generators.Client;

internal class AddClientDIExtensions : IMiddleware<ApiGenContext>
{
    public void Execute(ApiGenContext context)
    {
        var builder = new StringBuilder();
        var writer = new IndentedWriter(builder);

        var config = context.Options;
        var symbol = context.Symbols;
        var conventions = config.ClientConventions;
        
        var serviceCollection = symbol.IServiceCollection.ToDisplayString();
        var actionName = symbol.Action.ToDisplayString();
        var apiClientOptionsName = "ApiClientOptions";
        var configureOptionsArgName = $"{actionName}<{apiClientOptionsName}>";
        var serviceDescriptorName = symbol.ServiceDescriptor.ToDisplayString();
        var serviceLifetimeName = symbol.ServiceLifetime.ToDisplayString();
            
        writer.WriteHeaderLines();
        writer.WriteLine($"namespace {conventions.ExtensionsNamespace}");
        writer.WriteBlock(nsw =>
        {
            nsw.WriteLine("public static class DependencyInjectionExtensions");
            nsw.WriteBlock(cw =>
            {
                cw.WriteLine($"public static {serviceCollection} Add{config.ApiName}(this {serviceCollection} services, {configureOptionsArgName} configureOptions = default)");
                cw.WriteBlock(mw =>
                {
                    mw.WriteLine("configureOptions ??= _ => {  };");
                    mw.WriteLine($"var options = new {apiClientOptionsName}();");
                    mw.WriteLine("configureOptions(options);");
                        
                    foreach (var service in context.Services.Select(t=>t.Service))
                    {
                        var api = conventions.GetApiService(service);
                        var client = conventions.GetClientService(service);
                        
                        mw.WriteLine($"services.Add(new {serviceDescriptorName}(typeof({api.Namespace}.{api.Name}), typeof({client.Namespace}.{client.Name}), options.ServiceLifetime));");
                    }
                        
                    mw.WriteLine();
                    mw.WriteLine("return services;");
                });
            });
            nsw.WriteLine();
            nsw.WriteLine($"public class {apiClientOptionsName}");
            nsw.WriteBlock(mw =>
            {
                mw.WriteLine($"public {serviceLifetimeName} ServiceLifetime {{ get; set; }} = {serviceLifetimeName}.Transient;");
            });
        });

        var sourceCode = new ApiGenSourceCode
        {
            Name = $"{conventions.ExtensionsNamespace}.DependencyInjectionExtensions.g.cs",
            Source = builder.ToString()
        };
        context.Result.Add(sourceCode);
    }
}
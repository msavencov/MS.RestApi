using System.Linq;
using System.Text;
using MS.RestApi.SourceGenerator.Builder;
using MS.RestApi.SourceGenerator.Extensions;
using MS.RestApi.SourceGenerator.Pipe;
using MS.RestApi.SourceGenerator.Utils;

namespace MS.RestApi.SourceGenerator.Client
{
    internal class AddClientDIExtensions : IMiddleware<ApiGenContext>
    {
        public void Execute(ApiGenContext context)
        {
            var services = context.Requests.Select(t => t.EndPoint.Service).Distinct();

            var builder = new StringBuilder();
            var writer = new IndentedWriter(builder);

            var config = context.Config;
            var symbol = context.KnownSymbols;
            var serviceCollectionName = symbol.IServiceCollection.FullName();
            var actionName = symbol.Action.FullName();
            var apiClientOptionsName = "ApiClientOptions";
            var configureOptionsArgName = $"{actionName}<{apiClientOptionsName}>";
            var argumentNullExceptionName = symbol.ArgumentNullException;
            var serviceDescriptorName = symbol.ServiceDescriptor.FullName();
            var requestHandlerName = symbol.ClientRequestHandlerInterface.FullName();
            var serviceLifetimeName = symbol.ServiceLifetime.FullName();
            
            writer.WriteLine($"namespace {config.ClientExtensionsNamespace}");
            writer.WriteBlock(nsw =>
            {
                nsw.WriteLine("public static class DependencyInjectionExtensions");
                nsw.WriteBlock(cw =>
                {
                    cw.WriteLine($"public static {serviceCollectionName} Add{config.ApiName}Api(this {serviceCollectionName} services, {configureOptionsArgName} configureOptions = default)");
                    cw.WriteBlock(mw =>
                    {
                        mw.WriteLine($"configureOptions ??= _ => {{  }};");
                        mw.WriteLine($"var options = new {apiClientOptionsName}();");
                        mw.WriteLine($"configureOptions(options);");
                        
                        foreach (var service in services)
                        {
                            var serviceName = $"{config.ClientRootNamespace}.{ApiGenRequest.BuildInterfaceName(service)}";
                            var serviceImplName = $"{config.ClientServicesImplNamespace}.{ApiGenRequest.BuildClientName(service)}";
                            
                            mw.WriteLine($"services.Add(new {serviceDescriptorName}(typeof({serviceName}), typeof({serviceImplName}), options.ServiceLifetime));");
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
                Name = $"{config.ClientExtensionsNamespace}.Module.cs",
                Source = builder.ToString()
            };
            context.SourceCode.Add(sourceCode);
        }
    }
}
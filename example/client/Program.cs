using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using client.GeneratedApi.Extensions;
using client.GeneratedApi.Services;
using Microsoft.Extensions.DependencyInjection;
using contract.Account;
using MS.RestApi.Client;

[assembly: MS.RestApi.ApiGenOptions(ContractAssembly = "contract", ApiName = "GeneratedApi", RootNamespace = "client", GenerateClient = true)]

namespace client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = BuildServiceProvider();
            var accountApi = services.GetRequiredService<IAccountApi>();

            var model = new SignInLocal
            {
                Password = "dasdasd",
                Username = "ad@ad@ad",
            };
            try
            {
                var result = await accountApi.SignInLocal(model, CancellationToken.None);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static IServiceProvider BuildServiceProvider(IServiceCollection services = default)
        {
            services ??= new ServiceCollection();
            services.AddHttpClient<IGeneratedApiRequestHandler, DefaultRequestHandler>()
                    .ConfigureHttpClient(client =>
                    {
                        client.BaseAddress = new Uri("http://localhost:5269/api/");
                    });
            services.AddGeneratedApi(options =>
            {
                options.ServiceLifetime = ServiceLifetime.Transient;
            });
            
            return services.BuildServiceProvider();
        }
    }
    
    internal class DefaultRequestHandler : RequestHandlerBase, IGeneratedApiRequestHandler
    {
        public DefaultRequestHandler(HttpClient client) : base(client)
        {
            
        }
    }
}
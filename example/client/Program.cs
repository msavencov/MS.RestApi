using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using client.GeneratedApi;
using client.GeneratedApi.Extensions;
using client.GeneratedApi.Services;
using Microsoft.Extensions.DependencyInjection;
using contract.Account;
using MS.RestApi.Client;
using MS.RestApi.Client.Exceptions;

namespace client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = BuildServiceProvider();
            var accountApi = services.GetRequiredService<IAccountApi>();
            try
            {
                var result1 = await accountApi.SignInLocalAsync(new SignInLocal(), CancellationToken.None);
                await accountApi.SignOutAsync(new SignOut());
            }
            catch (ApiRemoteErrorException)
            {
                // handle API error
            }
        }

        private static IServiceProvider BuildServiceProvider(IServiceCollection services = default)
        {
            services ??= new ServiceCollection();
            services.AddHttpClient<IGeneratedApiRequestHandler, DefaultRequestHandler>()
                    .ConfigureHttpClient(client =>
                    {
                        client.BaseAddress = new Uri("http://localhost:5269");
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

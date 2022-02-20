using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using client.Generated.Client.Abstractions;
using client.Generated.Generated.Client.Extensions;
using contract;
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
                var result = await accountApi.SigninAsync(new Signin(), CancellationToken.None);
            }
            catch (ApiRemoteErrorException errorException)
            {
                // handle API error
            }
        }

        private static IServiceProvider BuildServiceProvider(IServiceCollection services = default)
        {
            services ??= new ServiceCollection();
            services.AddHttpClient<IGeneratedRequestHandler, DefaultRequestHandler>()
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
    
    internal class DefaultRequestHandler : RequestHandlerBase, IGeneratedRequestHandler
    {
        public DefaultRequestHandler(HttpClient client) : base(client)
        {
            
        }
    }
}

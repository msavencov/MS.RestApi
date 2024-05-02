using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using client.GeneratedApi;
using client.GeneratedApi.Extensions;
using client.GeneratedApi.Services;
using Microsoft.Extensions.DependencyInjection;
using contract.Account;
using MS.RestApi.Abstractions;
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

            FileInfoAttachment fi = new FileInfo(@"C:\Users\maxim.savencov\Documents\DAAC\Procur\ДТЗ РКП.docx");
            FileInfoAttachment fi2 = new FileInfo(@"C:\Users\maxim.savencov\Documents\DAAC\Procur\ДТЗ-РКП-MS.docx");
            FileInfoAttachment fi3 = new FileInfo(@"C:\Users\maxim.savencov\Documents\DAAC\Procur\ДТЗ-РКП-MS-Answers.docx");
            var model = new Profile
            {
                Id = 1,
                Name = "sadsad",
                Avatar = fi,
                Documents = {fi2, fi3}, 
                Inner = new ProfileData
                {
                    Name = "2312131",
                    Doc = fi,
                    AttachmentsCollection = {fi, fi3}
                }
            };
            try
            {
                var result = await accountApi.Profile(model, CancellationToken.None);
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
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

[assembly: MS.RestApi.ApiGenOptions(ContractAssembly = "contract", ApiName = "MyApi", RootNamespace = "My.App", GenerateControllers = true, UseMediatorHandlers = true, GenerateServices = true)]

namespace server
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}
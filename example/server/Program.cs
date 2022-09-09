using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

[assembly: MS.RestApi.SourceGenerator.ApiGenConfig("AssemblyToScan", new []{"contract"})]
[assembly: MS.RestApi.SourceGenerator.ApiGenConfig("GenerateControllers", true)]
[assembly: MS.RestApi.SourceGenerator.ApiGenConfig("UseMediatorHandlers", true)]

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
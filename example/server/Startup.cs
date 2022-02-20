using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using MS.RestApi.Server;
using server.Generated.Services.Abstractions;
using server.Services;

namespace server
{
    internal class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                    .AddApiMvcOptions();
            services.AddScoped<IAccountService, AccountService>();

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "My API", Version = "v1"});
                c.IncludeXmlComments(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "server.xml"));
                c.IncludeXmlComments(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "contract.xml"));
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseEndpoints(builder =>
            {
                builder.MapControllers();
                builder.MapDefaultControllerRoute();
            });
            app.UseSwagger();
            app.UseSwaggerUI();
        }
    }
}
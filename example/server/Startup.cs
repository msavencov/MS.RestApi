using System.Xml.XPath;
using contract;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using MS.RestApi.Server;
using MS.RestApi.Server.Extensions;
using MS.RestApi.Server.Swagger;

namespace server
{
    internal class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddControllers();
            services.AddApiMvcOptions();
            services.AddMediatR(configuration =>
            {
                configuration.RegisterServicesFromAssembly(typeof(Startup).Assembly);
            });
            
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo {Title = "My API", Version = "v1"});

                var docs = new[]
                {
                    typeof(Module).Assembly.GetDocumentation(),
                    typeof(Startup).Assembly.GetDocumentation(),
                };
                
                foreach (var doc in docs.ReplaceInheritedComments())
                {
                    options.IncludeXmlComments(() => new XPathDocument(doc.CreateReader()));
                }
                //options.OperationFilter<AttachmentOperationFilterFilter>();
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
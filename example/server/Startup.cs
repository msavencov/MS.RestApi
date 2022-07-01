using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
            services.AddControllers();
            services.AddApiMvcOptions();
            services.AddMediatR(typeof(Startup));
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "My API", Version = "v1"});

                var docs = new[]
                {
                    typeof(contract.Module).Assembly.GetDocumentationFilePath(),
                    typeof(server.Startup).Assembly.GetDocumentationFilePath(),
                }.Select(XDocument.Load).ToArray();
                
                foreach (var doc in docs.ReplaceInheritedComments())
                {
                    c.IncludeXmlComments(() => new XPathDocument(doc.CreateReader()));
                }
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
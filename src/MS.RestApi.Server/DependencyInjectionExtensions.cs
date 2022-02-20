using Microsoft.Extensions.DependencyInjection;
using MS.RestApi.Server.Exceptions;
using MS.RestApi.Server.Filters;
using Newtonsoft.Json.Serialization;

namespace MS.RestApi.Server
{
    public static class DependencyInjectionExtensions
    {
        public static IMvcBuilder AddApiMvcOptions(this IMvcBuilder builder)
        {
            builder.AddMvcOptions(options =>
                {
                    options.Filters.Add<ExceptionHandlerFilterAttribute>();
                })
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        throw new InvalidModelStateException(context.ModelState);
                    };
                });
            
            return builder;
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using MS.RestApi.Server.Exceptions;
using MS.RestApi.Server.Filters;

namespace MS.RestApi.Server;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApiMvcOptions(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.Configure<MvcOptions>(options =>
        {
            options.Filters.Add<ExceptionHandlerFilterAttribute>();
        });
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context => throw new InvalidModelStateException(context.ModelState);
        });
        services.AddTransient<IApiDescriptionProvider, CustomApiDescriptionProvider>();
        
        return services;
    }
}
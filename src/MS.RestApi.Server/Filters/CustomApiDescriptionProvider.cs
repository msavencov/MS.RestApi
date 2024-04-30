using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MS.RestApi.Abstractions;

namespace MS.RestApi.Server.Filters;

internal class CustomApiDescriptionProvider(IModelMetadataProvider metadataProvider) : IApiDescriptionProvider
{
    public void OnProvidersExecuting(ApiDescriptionProviderContext context)
    {
        foreach (var apiDescription in context.Results)
        {
            foreach (var parameterDescription in apiDescription.ParameterDescriptions)
            {
                if (parameterDescription.Type == typeof(IAttachment))
                {
                    parameterDescription.Type = typeof(IFormFile);
                    parameterDescription.ModelMetadata = metadataProvider.GetMetadataForType(typeof(IFormFile));
                    continue;
                }

                if (IsEnumerable<IAttachment>(parameterDescription.Type))
                {
                    parameterDescription.Type = typeof(IFormFile[]);
                    parameterDescription.ModelMetadata = metadataProvider.GetMetadataForType(typeof(IFormFile[]));
                    continue;
                }
            }
        }
    }

    private bool IsEnumerable<T>(Type type)
    {
        return typeof(IEnumerable<T>).IsAssignableFrom(type);
    }

    public void OnProvidersExecuted(ApiDescriptionProviderContext context)
    {
        
    }

    public int Order { get; } = 0;
}
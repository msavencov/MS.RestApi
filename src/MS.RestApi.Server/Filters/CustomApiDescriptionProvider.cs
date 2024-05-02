using System;
using System.Collections.Generic;
using System.Linq;
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
            var attachments =
            (
                from pd in apiDescription.ParameterDescriptions
                where pd.ModelMetadata.ContainerType == typeof(IAttachment)
                select pd
            ).ToList();
            
            foreach (var parameterDescription in apiDescription.ParameterDescriptions)
            {
                if (typeof(IAttachment).IsAssignableFrom(parameterDescription.Type))
                {
                    parameterDescription.Type = typeof(IFormFile[]);
                    parameterDescription.ModelMetadata = metadataProvider.GetMetadataForType(typeof(IFormFile[]));
                    continue;
                }
                
                if (typeof(IEnumerable<IAttachment>).IsAssignableFrom(parameterDescription.Type))
                {
                    parameterDescription.Type = typeof(IFormFile);
                    parameterDescription.ModelMetadata = metadataProvider.GetMetadataForType(typeof(IFormFile));
                    continue;
                }
            }
        }
    }

    public void OnProvidersExecuted(ApiDescriptionProviderContext context)
    {
        
    }

    public int Order { get; } = 0;
}
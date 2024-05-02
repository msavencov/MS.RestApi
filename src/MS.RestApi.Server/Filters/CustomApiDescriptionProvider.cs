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
                let name = pd.Name[.. pd.Name.LastIndexOf('.')]
                select (name, pd, pd.ModelMetadata.ContainerType)
            ).ToList();

            foreach (var attachment in attachments)
            {
                apiDescription.ParameterDescriptions.Remove(attachment.pd);
            }

            foreach (var (name, containerType) in attachments.Select(t=>(t.name, t.ContainerType)).Distinct())
            {
                var parameterDescription = new ApiParameterDescription
                {
                    Name = name,
                    Type = containerType,
                    ModelMetadata = metadataProvider.GetMetadataForType(containerType),
                    Source = BindingSource.Form,
                };
                apiDescription.ParameterDescriptions.Add(parameterDescription);
            }
            
            foreach (var parameterDescription in apiDescription.ParameterDescriptions)
            {
                if (typeof(IAttachment).IsAssignableFrom(parameterDescription.Type))
                {
                    parameterDescription.Type = typeof(IFormFile[]);
                    parameterDescription.ModelMetadata = metadataProvider.GetMetadataForType(typeof(IFormFile[]));
                } 
                else if (typeof(IEnumerable<IAttachment>).IsAssignableFrom(parameterDescription.Type))
                {
                    parameterDescription.Type = typeof(IFormFile);
                    parameterDescription.ModelMetadata = metadataProvider.GetMetadataForType(typeof(IFormFile)); 
                }
            }
        }
    }

    private bool IsAttachment(Type type)
    {
        return typeof(IAttachment).IsAssignableFrom(type) || typeof(IEnumerable<IAttachment>).IsAssignableFrom(type);
    }

    public void OnProvidersExecuted(ApiDescriptionProviderContext context)
    {
        
    }

    public int Order { get; } = -900; // between DefaultApiDescriptionProvider and EndpointMetadataApiDescriptionProvider
}
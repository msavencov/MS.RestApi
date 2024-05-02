using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MS.RestApi.Abstractions;

namespace MS.RestApi.Server.Filters;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class BindFormFileAttribute(string parameterName, string propertyName) : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var model = context.ActionArguments[parameterName];
        var modelType = model.GetType();
        var modelProperty = modelType.GetProperty(propertyName)!;
        var formFiles = context.HttpContext.Request.Form.Files;
        
        if (typeof(IEnumerable<IAttachment>).IsAssignableFrom(modelProperty.PropertyType))
        {
            var files = formFiles.Where(t => t.Name == modelProperty.Name).Select(t => new FromFileAttachment(t));
            if (modelProperty.GetValue(model, []) is ICollection<IAttachment> modelAttachments)
            {
                foreach (var file in files)
                {
                    modelAttachments.Add(file);
                }
            }
            else
            {
                modelProperty.SetValue(model, new List<IAttachment>(files));
            }
        }

        if (modelProperty.PropertyType == typeof(IAttachment))
        {
            if (formFiles.FirstOrDefault(t => t.Name == modelProperty.Name) is var file)
            {
                modelProperty.SetValue(model, new FromFileAttachment(file));
            }
        }
        
        await next();
    }

    private class FromFileAttachment(IFormFile file) : IAttachment
    {
        public string GetFileName() => file.Name;
        public string? GetContentType() => file.ContentType ?? "application/octet-stream";
        public long? GetContentLength() => file.Length;
        public Stream GetFileStream() => file.OpenReadStream();
    }
}

public class MultipartModel : Attribute
{
    
}

public class MultipartModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        return Task.CompletedTask;
    }
}
public class MultipartModelBinderProvider : IModelBinderProvider
{
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        return null;
    }
}

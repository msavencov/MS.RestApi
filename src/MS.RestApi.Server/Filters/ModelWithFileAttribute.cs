using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using MS.RestApi.Abstractions;

namespace MS.RestApi.Server.Filters;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class BindFormFileAttribute(string modelPropertyName, string parameterName) : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var model = context.ActionArguments[parameterName];
        var modelType = model.GetType();
        var modelProperty = modelType.GetProperty(modelPropertyName)!;
        var http = context.HttpContext;
        
        if (modelProperty.PropertyType == typeof(AttachmentsCollection))
        {
            var files = http.Request.Form.Files.Where(t => t.Name == modelProperty.Name).Select(t => new FromFileAttachment(t));
            if (modelProperty.GetValue(model, []) is AttachmentsCollection modelAttachments)
            {
                modelAttachments.AddRange(files);
            }
            else
            {
                modelProperty.SetValue(model, new AttachmentsCollection(files));
            }
        }

        if (modelProperty.PropertyType == typeof(IAttachment))
        {
            if (http.Request.Form.Files.FirstOrDefault(t => t.Name == modelProperty.Name) is var file)
            {
                modelProperty.SetValue(model, new FromFileAttachment(file));
            }
        }
        
        await next();
    }

    private class FromFileAttachment(IFormFile file) : IAttachment
    {
        public string FileName { get; set; } = file.FileName;
        public string ContentType { get; set; } = file.ContentType;
        public long ContentLength { get; set; } = file.Length;
        
        public Stream GetFileStream()
        {
            return file.OpenReadStream();
        }
    }
}


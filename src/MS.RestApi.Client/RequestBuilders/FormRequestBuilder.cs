using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using MS.RestApi.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MS.RestApi.Client.RequestBuilders;

public class FormRequestBuilder(RequestFactory factory, object model, Dictionary<string, object> attachments) : IRequestBuilder
{
    public HttpRequestMessage GetRequestMessage()
    {
        var content = new MultipartFormDataContent();
        var serializer = new JsonSerializerSettings
        {
            Converters = { new AttachmentConverter() }
        };
        var json = JsonConvert.SerializeObject(model, serializer);
        
        content.Add(new StringContent(json, Encoding.UTF8, "application/json"));
        
        return factory.CreateRequestMessage(content);
    }

    private void AddAttachments(MultipartFormDataContent content, JToken value)
    {
        
    }

    private void AddProperties(MultipartFormDataContent form, object value, string path = "")
    {
        foreach (var property in value.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (TryAddContent(form, property.GetValue(value), $"{path}{property.Name}"))
            {
                continue;
            }
        }
    }
    
    
    private bool TryAddContent(MultipartFormDataContent form, object? value, string path)
    {
        if (value is null)
        {
            return false;
        }

        var type = value.GetType();
        
        if (value is string s)
        {
            form.Add(new StringContent(s), path);
            return true;
        }

        if (IsDate(type))
        {
            form.Add(new StringContent(((IFormattable)value).ToString("o", CultureInfo.InvariantCulture)), path);
            return true;
        }

        if (value is IFormattable formattable)
        {
            form.Add(new StringContent(formattable.ToString(null, CultureInfo.InvariantCulture)), path);
            return true;
        }

        if (value is IAttachment attachment)
        {
            var content = new StreamContent(attachment.GetFileStream());
            
            if (attachment.GetContentLength() is {} contentLength)
            {
                content.Headers.ContentLength = contentLength;
            }

            if (attachment.GetContentType() is { } contentType)
            {
                content.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
            }
            
            form.Add(content, path, attachment.GetFileName());
            return true;
        }
        
        if (value is IEnumerable enumerable)
        {
            var i = 0; 
            foreach (var inner in enumerable)
            {
                TryAddContent(form, inner, path);
            }

            return true;
        }

        return false;
    }

    private bool IsDate(Type type)
    {
        if (Nullable.GetUnderlyingType(type) is { } underlyingType)
        {
            type = underlyingType;
        }
        
        if (type == typeof(DateTime) || type == typeof(DateTimeOffset) || type.FullName == "System.DateOnly")
        {
            return true;
        }
        
        return false;
    }

    private bool IsNumeric(Type type)
    {
        var typeCode = Type.GetTypeCode(type);

        switch (typeCode)
        {
            case TypeCode.Byte:
            case TypeCode.SByte:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
            case TypeCode.Decimal:
            case TypeCode.Double:
            case TypeCode.Single:
            {
                return true;
            }
            default:
            {
                return false;
            }
        }
    }
}
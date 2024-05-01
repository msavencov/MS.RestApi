using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using MS.RestApi.Abstractions;

namespace MS.RestApi.Client.RequestBuilders;

public class FormRequestBuilder(RequestFactory factory, object model, Dictionary<string, object> attachments) : IRequestBuilder
{
    public HttpRequestMessage GetRequestMessage()
    {
        var content = new MultipartFormDataContent();
        var type = model.GetType();

        AddProperties(content, model);
        
        
        
        
        return factory.CreateRequestMessage(content);
    }

    private void AddProperties(MultipartFormDataContent content, object input)
    {
        foreach (var property in input.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            var value = property.GetValue(model);
            var attachments = new List<(string file, StreamContent content)>();
            
            if (value is IAttachment attachment)
            {
                var streamContent = new StreamContent(attachment.GetFileStream() )
                {
                    Headers =
                    {
                        ContentType = new MediaTypeHeaderValue(attachment.ContentType),
                        ContentLength = attachment.ContentLength,
                    }
                };
                attachments.Add((attachment.FileName, streamContent));
            }
            
            \\\
            
            foreach (var item in GetHttpContent(value))
            {
                content.Add(item);
            }
        }
    }

    private IEnumerable<HttpContent> GetHttpContent(object? value)
    {
        if (value is null)
        {
            yield break;
        }
     
        var propertyType = value.GetType();
            
        if (value is string s)
        {
            yield return new StringContent(s);
        }

        if (IsDate(propertyType))
        {
            yield return new StringContent(((IFormattable)value).ToString("o", CultureInfo.InvariantCulture));
        }

        if (value is IFormattable formattable)
        {
            yield return new StringContent(formattable.ToString(null, CultureInfo.InvariantCulture));
        }

        if (value is IAttachment attachment)
        {
            yield return new StreamContent(attachment.GetFileStream());
        }
        
        if (value is IEnumerable enumerable)
        {
            foreach (var inner in enumerable)
            {
                foreach (var innerContent in GetHttpContent(inner))
                {
                    yield return innerContent;
                }
            }
        }
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
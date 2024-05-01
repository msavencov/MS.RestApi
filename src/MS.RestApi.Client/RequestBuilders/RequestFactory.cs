using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Web;

namespace MS.RestApi.Client.RequestBuilders;

public class RequestFactory(string method, string route, Dictionary<string, object> parameters)
{
    public HttpRequestMessage CreateRequestMessage(HttpContent content)
    {
        foreach (var parameter in parameters)
        {
            var key = $"{{{parameter.Key}}}";
            var value = Convert.ToString(parameter.Value, CultureInfo.InvariantCulture);
            
            route = route.Replace(key, HttpUtility.UrlEncode(value));
        }

        return new HttpRequestMessage(new HttpMethod(method.ToUpper()), route)
        {
            Content = content,
        };
    }
}
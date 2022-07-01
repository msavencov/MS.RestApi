using System;
using System.Net.Http;

namespace MS.RestApi.Client.Exceptions;

public class ApiClientException : Exception
{
    public ApiClientException(string message, HttpResponseMessage response, Exception innerException = null) : base(message, innerException)
    {
        Response = response;
    }

    public HttpResponseMessage Response { get; }
}
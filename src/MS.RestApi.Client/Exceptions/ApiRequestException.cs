using System;
using System.Net.Http;
using MS.RestApi.Client.Extensions;

namespace MS.RestApi.Client.Exceptions;

public class ApiRequestException : ApiClientException
{
    internal ApiRequestException(string message) : base(message)
    {
    }
    
    public HttpRequestMessage Request { get; init; }
    public HttpResponseMessage Response { get; init; }
}
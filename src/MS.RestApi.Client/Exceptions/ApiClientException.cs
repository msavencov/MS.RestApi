using System;

namespace MS.RestApi.Client.Exceptions;

public class ApiClientException : Exception
{
    internal ApiClientException()
    {
    }

    internal ApiClientException(string message) : base(message)
    {
    }

    internal ApiClientException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
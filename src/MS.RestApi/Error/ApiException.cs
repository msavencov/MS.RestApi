using System;

namespace MS.RestApi.Error;

public abstract class ApiException : Exception
{
    public abstract ApiError ApiError { get; } 
        
    protected ApiException(string message) : base(message)
    {
    }

    protected ApiException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public override string ToString()
    {
        return $"{GetType().FullName}: {ApiError}{Environment.NewLine}{StackTrace}";
    }
}
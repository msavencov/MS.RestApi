using System;

namespace MS.RestApi.Error
{
    public abstract class ApiException : Exception
    {
        public ApiError Error { get; init; }
    }
}
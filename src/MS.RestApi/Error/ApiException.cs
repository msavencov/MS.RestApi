using System;

namespace MS.RestApi.Error
{
    public abstract class ApiException : Exception
    {
        protected ApiException(string message) : base(message)
        {
        }

        protected ApiException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ApiError Error { get; protected set; }
    }
}
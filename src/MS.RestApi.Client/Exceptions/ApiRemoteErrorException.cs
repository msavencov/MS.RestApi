using System;
using MS.RestApi.Error;

namespace MS.RestApi.Client.Exceptions
{
    public class ApiRemoteErrorException : ApiException
    {
        public ApiRemoteErrorException(string message) : base(message)
        {
        }

        public ApiRemoteErrorException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
using MS.RestApi.Error;

namespace MS.RestApi.Client.Exceptions
{
    public class ApiRemoteErrorException : ApiException
    {
        public ApiRemoteErrorException(string message, ApiError error) : base(message)
        {
            Error = error;
        }
    }
}
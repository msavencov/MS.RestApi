using MS.RestApi.Error;

namespace MS.RestApi.Client.Exceptions;

public class ApiErrorException : ApiException
{
    public override ApiError ApiError { get; }

    internal ApiErrorException(ApiError error) : base($"API error occured: [{error.Code}] {error.Reason}")
    {
        ApiError = error;
    }
}
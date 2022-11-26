using MS.RestApi.Error;

namespace MS.RestApi.Errors;

public class GenericApiError : ApiError
{
    public override string Code { get; } = "Generic";
}
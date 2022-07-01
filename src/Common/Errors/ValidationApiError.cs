using System.Collections.Generic;
using MS.RestApi.Error;

namespace MS.RestApi.Errors;

public class ValidationApiError : ApiError<Dictionary<string, string[]>>
{
    public override string Code { get; } = "Validation";
}
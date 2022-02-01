using System.Collections.Generic;

namespace MS.RestApi.Error.BadRequest
{
    public class ValidationApiError : ApiError
    {
        public Dictionary<string, string[]> ValidationErrors { get; init; }
    }
}
using System.Collections.Generic;
using MS.RestApi.Error;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MS.RestApi.Errors;

public class GenericApiError : ApiError
{
    public override string Code { get; } = "Generic";

    [JsonExtensionData] 
    private Dictionary<string, JToken> ExtensionData { get; set; } = new();

    public override string ToString()
    {
        return base.ToString() + ExtensionData;
    }
}
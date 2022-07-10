using System.Collections.Generic;
using MS.RestApi.Error;
using Newtonsoft.Json;

namespace MS.RestApi.Errors;

public class GenericApiError : ApiError
{
    public override string Code { get; } = "Generic";

    [JsonExtensionData] 
    private Dictionary<string, JsonToken> ExtensionData { get; set; } = new();
}
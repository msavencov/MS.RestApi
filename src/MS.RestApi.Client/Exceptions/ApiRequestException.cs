using System;
using System.Linq;
using MS.RestApi.Client.Extensions;

namespace MS.RestApi.Client.Exceptions;

public class ApiRequestException : ApiClientException
{
    internal ApiRequestException(string message) : base(message)
    {
    }

    public string RequestUrl { get; internal set; }
    public string RequestBody { get; internal set; }
    public int ResponseCode { get; internal set; }
    public string ResponsePhrase { get; internal set; }
    public string ResponseBody { get; internal set; }

    public override string ToString()
    {
        var nl = Environment.NewLine;
        var log = new[]
        {
            $"HTTP Request Url: {RequestUrl}",
            $"HTTP Request Body: {RequestBody}",
            $"HTTP Response Code: {ResponseCode}",
            $"HTTP Response Phrase: {ResponsePhrase}",
            $"HTTP Response Body: {ResponseBody}",
        };
        return $"{base.ToString()}{nl}{log.Join(nl)}";
    }
}
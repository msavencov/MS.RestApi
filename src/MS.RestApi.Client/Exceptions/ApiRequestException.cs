using System;

namespace MS.RestApi.Client.Exceptions;

public class ApiRequestException : ApiClientException
{
    internal ApiRequestException()
    {
    }

    public string RequestUrl { get; internal set; }
    public int ResponseCode { get; internal set; }
    public string ResponsePhrase { get; internal set; }
    public string ResponseBody { get; internal set; }

    public override string ToString()
    {
        var nl = Environment.NewLine;
        var log = new[]
        {
            $"HTTP Request Url: {RequestUrl}",
            $"HTTP Response Code: {ResponseCode}",
            $"HTTP Response Phrase: {ResponsePhrase}",
            $"HTTP Response Body: {ResponseBody}",
        };
        return $"{GetType().FullName}: {nl}{string.Join(Environment.NewLine, log)}{nl}{StackTrace}";
    }
}
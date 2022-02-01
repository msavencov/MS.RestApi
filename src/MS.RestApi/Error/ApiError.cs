using System;

namespace MS.RestApi.Error
{
    public class ApiError
    {
        public int Code { get; init; }
        public string CodeName { get; init; }
        public string MessageFormat { get; init; }
        public string[] MessageFormatArgs { get; init; } = Array.Empty<string>();
        public string LogMessage { get; init; }
    }
}
using System;

namespace MS.RestApi.Error
{
    public class ApiError
    {
        public int Code { get; init; }
        public string CodeName { get; init; }
        public string ErrorMessage { get; init; }
        public string LogMessage { get; init; }
    }
}
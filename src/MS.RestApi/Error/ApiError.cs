using System;

namespace MS.RestApi.Error
{
    public class UnhandledApiError : ApiError
    {
        public override string Code { get; } = "Generic";
    }
    
    public abstract class ApiError
    {
        public abstract string Code { get; }
        public string Reason { get; set; }
        public string LogMessage { get; set; }
    }

    public abstract class ApiError<TDetail> : ApiError where TDetail : new()
    {
        public TDetail Detail { get; set; } = new TDetail();
    }
}
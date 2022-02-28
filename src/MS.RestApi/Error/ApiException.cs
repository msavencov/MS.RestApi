using System;

namespace MS.RestApi.Error
{
    public abstract class ApiException : Exception
    {
        protected ApiException(string message) : base(message)
        {
        }

        protected ApiException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ApiError Error { get; init; }

        public override string ToString()
        {
            var nl = Environment.NewLine;
            return $"[{Error.CodeName}: {Error.Code}] {Error.ErrorMessage}{nl}{Error.LogMessage}{nl}{base.ToString()}";
        }
    }
}
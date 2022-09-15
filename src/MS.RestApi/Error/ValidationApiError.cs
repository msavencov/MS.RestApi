using System;
using System.Collections.Generic;
using System.Linq;

namespace MS.RestApi.Error;

public class ValidationApiError : ApiError<Dictionary<string, string[]>>
{
    public override string Code { get; } = "Validation";

    public override string ToString()
    {
        var nl = Environment.NewLine;
        var log = Detail.Aggregate("", (current, error) => current + $"{error.Key}: {nl} {string.Join(nl, error.Value.Select(t => $" * {t}"))}{nl}");
        
        return $"{base.ToString()}{nl}{log}";
    }
}
namespace MS.RestApi.Error;

public abstract class ApiError
{
    public abstract string Code { get; }
    public string Reason { get; set; }
    public string LogMessage { get; set; }

    public override string ToString()
    {
        return $"{GetType().Name}: {Code}: {Reason}";
    }
}

public abstract class ApiError<TDetail> : ApiError where TDetail : new()
{
    public TDetail Detail { get; set; } = new ();
}
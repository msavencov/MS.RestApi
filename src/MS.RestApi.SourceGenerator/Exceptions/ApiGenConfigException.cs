namespace MS.RestApi.SourceGenerator.Exceptions;

internal class ApiGenConfigException : ApiGenException
{
    public ApiGenConfigException(int id, string message) : base(100 + id, message)
    {
        Category = "Configuration";
    }
}
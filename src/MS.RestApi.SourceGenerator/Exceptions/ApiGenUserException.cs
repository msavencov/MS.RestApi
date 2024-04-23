namespace MS.RestApi.SourceGenerator.Exceptions;

internal class ApiGenUserException : ApiGenException
{
    public ApiGenUserException(int id, string message) : base(200 + id, message)
    {
        Category = "User";
    }
}
using System;

namespace MS.RestApi.SourceGenerator.Utils;

internal class ApiGenException : Exception
{
    public int Id { get; }
    public string Category { get; init; }

    public ApiGenException(int id, string message) : base(message)
    {
        Id = id;
    }
}
using Microsoft.CodeAnalysis;

namespace MS.RestApi.SourceGenerator.Extensions;

internal class DiagnosticError
{
    public DiagnosticError(int id, string message)
    {
        Id = $"ApiGen{id:0000}";
        Message = message;
    }

    public string Id { get; }
    public string Message { get; }
    public Location Location { get; set; }
    public string Category { get; set; } = "ApiGen";
}
using System;
using Microsoft.CodeAnalysis;

namespace MS.RestApi.SourceGenerator.Exceptions;

internal class ApiGenException(int id, string message) : Exception(message)
{
    public int Id { get; } = id;
    public string? Category { get; init; }

    public void ReportDiagnosticError(Action<Diagnostic> reportDiagnostic)
    {
        var id = $"ApiGen{Id:0000}";
        var category = Category ?? "ApiGen";
        
        var diagnostic = Diagnostic.Create(id, category, Message,
            defaultSeverity: DiagnosticSeverity.Error,
            severity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            warningLevel: 0,
            location: Location.None);

        reportDiagnostic(diagnostic);
    }
}
using System;
using Microsoft.CodeAnalysis;

namespace MS.RestApi.SourceGenerator.Exceptions;

internal class ApiGenException(int id, string message) : Exception(message)
{
    public int Id { get; } = id;
    public string Category { get; init; } = "ApiGen";

    public void ReportDiagnosticError(Action<Diagnostic> reportDiagnostic)
    {
        var id = $"ApiGen{Id:0000}";
        
        var diagnostic = Diagnostic.Create(id, Category, Message,
            defaultSeverity: DiagnosticSeverity.Error,
            severity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            warningLevel: 0,
            location: Location.None);

        reportDiagnostic(diagnostic);
    }

    public static ApiGenException RequiredAssemblyReference(string assembly)
    {
        return new ApiGenException(1, $"The '{assembly}' assembly should be referenced for client generator.");
    }
}
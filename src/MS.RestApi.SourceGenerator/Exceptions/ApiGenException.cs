using System;
using Microsoft.CodeAnalysis;

namespace MS.RestApi.SourceGenerator.Exceptions;

internal class ApiGenException : Exception
{
    private ApiGenException(int id, string message) : base(message)
    {
        Id = id;
    }

    public int Id { get; }
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
        return new ApiGenException(101, $"The '{assembly}' assembly should be referenced to project.")
        {
            Category = "User"
        };
    }
    
    public static ApiGenException EmbeddedResourceMissing(string resourceNameEndsWith)
    {
        return new ApiGenException(1, $"The resource with name ends with '{resourceNameEndsWith}' not found.")
        {
            Category = "Core",
        };
    }
}
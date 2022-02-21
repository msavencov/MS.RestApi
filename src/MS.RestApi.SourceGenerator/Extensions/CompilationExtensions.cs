using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using MS.RestApi.SourceGenerator.Utils;

namespace MS.RestApi.SourceGenerator.Extensions
{
    internal static class CompilationExtensions
    {
        private static string GetBuildProperty(this AnalyzerConfigOptions options, string propertyName)
        {
            if (options.TryGetValue($"build_property.{propertyName}", out var value))
            {
                return value;
            }

            return default;
        }

        private static string GetApiGenBuildProperty(this AnalyzerConfigOptions options, string propertyName)
        {
            return options.GetBuildProperty($"ApiGen{propertyName}");
        }

        public static string GetBuildProperty(this GeneratorExecutionContext context, string propertyName)
        {
            return context.AnalyzerConfigOptions.GlobalOptions.GetBuildProperty(propertyName);
        }
        
        public static string GetApiGenBuildProperty(this GeneratorExecutionContext context, string propertyName)
        {
            return context.AnalyzerConfigOptions.GlobalOptions.GetApiGenBuildProperty(propertyName);
        }

        public static Exception ReportTypeNotFoundDiagnosticError(this GeneratorExecutionContext context, string typeName, Location location = null)
        {
            return context.ReportDiagnosticError(new DiagnosticError(1, $"The type '{typeName}' was not found. Please, Check you project references.")
            {
                Category = "ApiGenTypeNotFound",
                Location = location,
            });
        }

        public static Exception ReportDiagnosticError(this GeneratorExecutionContext context, Exception exception)
        {
            var error = new DiagnosticError(1, exception.Message);

            if (exception is ApiGenException apiGenException)
            {
                error = new DiagnosticError(apiGenException.Id, apiGenException.Message);

                if (apiGenException.Category?.Length > 0)
                {
                    error.Category = apiGenException.Category;
                }
            }

            return context.ReportDiagnosticError(error);
        }

        public static Exception ReportDiagnosticError(this GeneratorExecutionContext context, int id, string message) => context.ReportDiagnosticError(new DiagnosticError(id, message));
        public static Exception ReportDiagnosticError(this GeneratorExecutionContext context, DiagnosticError error)
        {
            var diagnostic = Diagnostic.Create(
                error.Id,
                error.Category,
                error.Message,
                defaultSeverity: DiagnosticSeverity.Error,
                severity: DiagnosticSeverity.Error,
                isEnabledByDefault: true,
                warningLevel: 0,
                location: error.Location);

            context.ReportDiagnostic(diagnostic);

            return new Exception(error.Message);
        }

        public static IEnumerable<INamedTypeSymbol> GetNamespaceMembersRecursive(this INamespaceSymbol namespaceSymbol)
        {
            foreach (var typeMember in namespaceSymbol.GetTypeMembers())
            {
                yield return typeMember;
            }

            foreach (var namespaceMember in namespaceSymbol.GetNamespaceMembers())
            {
                foreach (var typeSymbol in namespaceMember.GetNamespaceMembersRecursive())
                {
                    yield return typeSymbol;
                }
            }
        }

        public static IEnumerable<INamedTypeSymbol> GetNamedTypeFromReferencedAssembly(this Compilation compilation, params string[] assemblyToScan)
        {
            foreach (var assemblyIdentity in compilation.SourceModule.ReferencedAssemblySymbols.Where(t => assemblyToScan.Contains(t.Name)))
            {
                foreach (var typeSymbol in assemblyIdentity.GlobalNamespace.GetNamespaceMembersRecursive())
                {
                    yield return typeSymbol;
                }
            }
        }

        public static string FullName(this ISymbol symbol)
        {
            return symbol.ToDisplayString();
        }
    }
}
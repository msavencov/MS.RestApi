using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using MS.RestApi.SourceGenerator.Descriptors;
using MS.RestApi.SourceGenerator.Exceptions;
using MS.RestApi.SourceGenerator.Generators.Client;
using MS.RestApi.SourceGenerator.Generators.Common;
using MS.RestApi.SourceGenerator.Generators.Server;
using MS.RestApi.SourceGenerator.Helpers.Pipe;

namespace MS.RestApi.SourceGenerator.Generators;

internal class ApiGenPipeline : Pipeline<ApiGenContext>
{
    public ApiGenPipeline(ApiGenContext context) : base(context)
    {
        Add<AddApiRequests>();
        
        if (context.Options.GenerateControllers is not GenerateControllers.None)
        {
            Add<AddControllers>();
        }

        if (context.Options.GenerateServices is GenerateServices.WithService)
        {
            Add<AddServices>();
        }
        
        if (context.Options.GenerateServices is GenerateServices.WithMediator)
        {
            Add<AddMediator>();
        }
        
        if (context.Options.GenerateClient)
        {
            Add<AddClientInterfaces>();
            Add<AddClientImplementation>();
            Add<AddClientDIExtensions>();
        }
    }

    internal static IEnumerable<ApiGenSourceCode> Run(ApiGenContext context, Action<Diagnostic> reportDiagnostic)
    {
        var pipeline = new ApiGenPipeline(context);
        
        try
        {
            pipeline.Run();
        }
        catch (ApiGenException exception)
        {
            exception.ReportDiagnosticError(reportDiagnostic);
        }

        return context.Result;
    }
}
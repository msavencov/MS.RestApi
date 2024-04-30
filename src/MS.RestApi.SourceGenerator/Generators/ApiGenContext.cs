using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using MS.RestApi.SourceGenerator.Descriptors;

namespace MS.RestApi.SourceGenerator.Generators;

internal record ApiGenContext
{
    public required Compilation Compilation { get; init; }
    public required ApiGenOptions Options { get; init; } = null!;
    public required KnownSymbols Symbols { get; init; } = null!;
    public CancellationToken CancellationToken { get; init; }

    public readonly Dictionary<string, HashSet<ApiRequestDescriptor>> Services = [];
    
    public HashSet<ApiGenSourceCode> Result { get; } = [];

    public void AddRequest(string service, ApiRequestDescriptor request)
    {
        if (Services.TryGetValue(service, out var requests))
        {
            requests.Add(request);
            return;
        }

        Services[service] = [request];
    }
}

internal record ApiGenSourceCode
{
    public required string Name { get; init; }
    public required string Source { get; init; }
}
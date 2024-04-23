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
    public required KnownSymbols KnownSymbols { get; init; } = null!;
    public CancellationToken CancellationToken { get; init; }

    public IEnumerable<(string Service, HashSet<ApiRequestDescriptor> Actions)> Services => _services ??= _requests.Select(t => (t.Key, t.Value)).ToList();
    private IEnumerable<(string Service, HashSet<ApiRequestDescriptor> Actions)>? _services;
    private readonly Dictionary<string, HashSet<ApiRequestDescriptor>> _requests = [];
    
    public HashSet<ApiGenSourceCode> Result { get; } = [];

    public void AddRequest(string service, ApiRequestDescriptor request)
    {
        if (_requests.TryGetValue(service, out var requests))
        {
            requests.Add(request);
            return;
        }

        _requests[service] = [request];
    }
}

internal record ApiGenService
{
    public required string Service { get; init; }
    public required HashSet<ApiRequestDescriptor> Requests { get; init; } = [];
}
internal record ApiGenSourceCode
{
    public required string Name { get; init; }
    public required string Source { get; init; }
}
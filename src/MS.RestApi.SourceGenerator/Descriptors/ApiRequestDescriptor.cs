using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using MS.RestApi.SourceGenerator.Generators;

namespace MS.RestApi.SourceGenerator.Descriptors;

internal record ApiRequestDescriptor
{
    public required string Service { get; init; }
    public required string Endpoint { get; init; }
    
    public required INamedTypeSymbol Request { get; init; } 
    public required INamedTypeSymbol? Response { get; init; }
    public string ReturnType => Response is null ? KnownSymbolNames.Task : $"{KnownSymbolNames.Task}<{Response.ToDisplayString()}>";
    
    private static readonly Regex ParseRouteArgumentsRegex = new (@"\{(?<param>\w+)\}", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private List<IPropertySymbol>? _routeArguments = null;
    public List<IPropertySymbol> GetRouteArguments()
    {
        if (_routeArguments is { })
        {
            return _routeArguments;
        }
        
        var matches = ParseRouteArgumentsRegex.Matches(Endpoint).OfType<Match>();
        var parameters = from match in matches.Select(t => t.Groups["param"].Value)
                         from property in Request.GetMembers().OfType<IPropertySymbol>()
                         where match == property.Name
                         select property;

        return _routeArguments = parameters.ToList();
    }
}
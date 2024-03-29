using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using MS.RestApi.SourceGenerator.Common;
using MS.RestApi.SourceGenerator.Extensions;
using MS.RestApi.SourceGenerator.Utils;

namespace MS.RestApi.SourceGenerator.Server.Helpers;

internal static class ControllerActionExtensions
{
    public static ControllerAction AsAction(this ApiGenRequest action, ApiGenContext context)
    {
        var symbols = context.KnownSymbols;
        var config = context.Config;
            
        return new ControllerAction(action)
        {
            HttpMethodAttribute = symbols.HttpPostAttribute.FullName(),
            HttpRouteAttribute = $"{symbols.RouteAttribute.FullName()}(\"{action.GetEndpointRoute(config)}\")",
            ResponseTypeName = action.GetResponseTypeName(context),
            ActionName = action.Request.Name,
            ModelFromAttributeName = symbols.FromBodyAttribute.FullName(),
            ModelTypeName = action.Request.FullName(),
            ResponseTypeIsVoid = SymbolEqualityComparer.Default.Equals(action.Response, symbols.Task)
        };
    }
    public static IEnumerable<ControllerAction> AsActions(this IEnumerable<ApiGenRequest> requests, ApiGenContext context)
    {
        return requests.Select(action => action.AsAction(context));
    }
}
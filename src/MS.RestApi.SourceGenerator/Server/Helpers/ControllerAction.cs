using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using MS.RestApi.Abstractions;
using MS.RestApi.SourceGenerator.Builder;
using MS.RestApi.SourceGenerator.Common;
using MS.RestApi.SourceGenerator.Extensions;
using MS.RestApi.SourceGenerator.Utils;

namespace MS.RestApi.SourceGenerator.Server.Helpers
{
    internal static class ControllerActionExtensions
    {
        public static IEnumerable<ControllerAction> AsActions(this IEnumerable<ApiGenRequest> requests, ApiGenContext context)
        {
            var symbols = context.KnownSymbols;
            var config = context.Config;
            
            foreach (var action in requests)
            {
                yield return new ControllerAction(action)
                {
                    HttpMethodAttribute = (action.EndPoint.Method switch
                    {
                        Method.Delete => symbols.HttpDeleteAttribute,
                        Method.Post => symbols.HttpPostAttribute,
                        _ => symbols.HttpGetAttribute,
                    }).FullName(),
                    HttpRouteAttribute = $"{symbols.RouteAttribute.FullName()}(\"{action.GetEndpointRoute(config)}\")",
                    ResponseTypeName = action.GetResponseTypeName(context),
                    ActionName = action.Request.Name,
                    ModelFromAttributeName = (action.EndPoint.Method switch
                    {
                        Method.Post => symbols.FromBodyAttribute,
                        _ => symbols.FromQueryAttribute,
                    }).FullName(),
                    ModelTypeName = action.Request.FullName(),
                    ResponseTypeIsVoid = SymbolEqualityComparer.Default.Equals(action.Response, symbols.Task)
                };
            }
        }
    }
    internal class ControllerAction
    {
        public ApiGenRequest Request { get; }

        public ControllerAction(ApiGenRequest request)
        {
            Request = request;
        }

        public string HttpRouteAttribute { get; init; }
        public string HttpMethodAttribute { get; init; }

        public string ActionName { get; init; }
        public string ResponseTypeName { get; init; }
        public string ModelFromAttributeName { get; init; }
        public string ModelTypeName { get; init; }
        public string ActionDescription { get; init; }
        public bool ResponseTypeIsVoid { get; init; }
    }
}
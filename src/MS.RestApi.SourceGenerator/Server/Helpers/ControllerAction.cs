using MS.RestApi.SourceGenerator.Utils;

namespace MS.RestApi.SourceGenerator.Server.Helpers;

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
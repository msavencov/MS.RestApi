using MS.RestApi.Abstractions;

namespace MS.RestApi.SourceGenerator.Tests.Templates.RouteParam;

[EndPoint($"test/{nameof(Id)}", "Test")]
public class Request : IApiRequest
{
    public int Id { get; set; }
}
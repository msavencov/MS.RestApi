using MediatR;
using MS.RestApi.Abstractions;

namespace Templates.RouteParam;

[EndPoint("test/{Id}", "Test")]
public class Request : IRequest<RequestResponse>
{
    public virtual required int Id { get; init; }
}
public class RequestResponse;
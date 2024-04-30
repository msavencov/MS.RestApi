using MediatR;
using MS.RestApi.Abstractions;

namespace Templates.RouteParam;

[EndPoint("route/parameter/record/{Id}/{Other}", "Test")]
public record RecordRequest : IRequest
{
    public required int Id { get; init; }
    public required int Other { get; init; }
}

[EndPoint("route/parameter/class/{Id}/{Other}", "Test")]
public class ClassRequest : IRequest
{
    public required int Id { get; set; }
    public required int Other { get; set; }
}



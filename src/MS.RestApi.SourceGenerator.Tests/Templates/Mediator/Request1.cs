using MediatR;
using MS.RestApi.Abstractions;

namespace Templates.Mediator;

[EndPoint("mediator/request1", "Test")]
public class Request1 : IRequest; 


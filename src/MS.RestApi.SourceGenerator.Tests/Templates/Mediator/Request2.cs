using MediatR;
using MS.RestApi.Abstractions;

namespace Templates.Mediator;

[EndPoint("mediator/request2", "Test")]
public class Request2 : IRequest<Request2Result>;
public class Request2Result; 


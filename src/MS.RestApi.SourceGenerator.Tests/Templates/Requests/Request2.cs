using MS.RestApi.Abstractions;

namespace Templates;

[EndPoint("route2", "Group")]
public class Request2 : MS.RestApi.Abstractions.IApiRequest<Request2Response>;
public class Request2Response;
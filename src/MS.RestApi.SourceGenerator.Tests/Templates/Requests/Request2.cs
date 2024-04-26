using MS.RestApi.Abstractions;

namespace Templates;

[EndPoint("route2", "Group")]
public class Request2 : MS.RestApi.Abstractions.Request<Request2Response>;
public class Request2Response;
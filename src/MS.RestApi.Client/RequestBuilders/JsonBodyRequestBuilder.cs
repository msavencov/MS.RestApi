using System.Net.Http;
using Newtonsoft.Json;

namespace MS.RestApi.Client.RequestBuilders;

public class JsonBodyRequestBuilder(RequestFactory factory, object model) : IRequestBuilder
{
    public HttpRequestMessage GetRequestMessage()
    {
        var serialized = JsonConvert.SerializeObject(model);
        var content = new JsonContent(serialized);

        return factory.CreateRequestMessage(content);
    }
}
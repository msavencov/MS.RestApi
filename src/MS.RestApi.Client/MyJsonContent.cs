using System.Net.Http;
using System.Net.Http.Headers;

namespace MS.RestApi.Client;

public class MyJsonContent : StringContent
{
    public MyJsonContent(string content) : base(content)
    {
        Headers.ContentType = new MediaTypeHeaderValue("application/json");
    }
}
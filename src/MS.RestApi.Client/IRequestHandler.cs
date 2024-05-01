using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MS.RestApi.Client;

public interface IRequestBuilder
{
    public HttpRequestMessage GetRequestMessage();
}

public interface IRequestHandler
{
    Task HandleAsync<TModel>(IRequestBuilder message, CancellationToken ct = default);
    Task<TResult> HandleAsync<TModel, TResult>(IRequestBuilder message, CancellationToken ct = default);
}
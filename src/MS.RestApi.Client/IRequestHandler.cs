using System.Threading;
using System.Threading.Tasks;

namespace MS.RestApi.Client;

public interface IRequestHandler
{
    Task HandleAsync<TModel>(string resource, TModel model, CancellationToken ct = default);
    Task<TResult> HandleAsync<TModel, TResult>(string resource, TModel model, CancellationToken ct = default);
}
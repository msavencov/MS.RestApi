using System.Threading;
using System.Threading.Tasks;
using MS.RestApi.Abstractions;

namespace MS.RestApi.Client
{
    public interface IRequestHandler
    {
        Task HandleAsync<TModel>(Method method, string resource, TModel model, CancellationToken ct = default);
        Task<TResult> HandleAsync<TModel, TResult>(Method method, string resource, TModel model, CancellationToken ct = default);
    }
}
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MS.RestApi.Abstractions;
using Newtonsoft.Json;

namespace MS.RestApi.Client
{
    public class RequestHandlerBase : IRequestHandler
    {
        protected readonly HttpClient Client;

        public RequestHandlerBase(HttpClient client)
        {
            Client = client;
        }
        
        public async Task HandleAsync<TModel>(Method method, string resource, TModel model, CancellationToken ct)
        {
            await ExecuteAsync(method, resource, model, ct);
        }

        public async Task<TResult> HandleAsync<TModel, TResult>(Method method, string resource, TModel model, CancellationToken ct)
        {
            var response = await ExecuteAsync(method, resource, model, ct);
            var responseBody = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();

            if (typeof(TResult) == typeof(string))
            {
                return (TResult)(object)responseBody;
            }
            
            try
            {
                return JsonConvert.DeserializeObject<TResult>(responseBody);
            }
            catch (Exception e)
            {
                await OnRequestExceptionAsync(response, e);
                throw;
            }
        }

        private async Task<HttpResponseMessage> ExecuteAsync(Method method, string resource, object model, CancellationToken ct)
        {
            var httpMethod = method switch
            {
                Method.Delete => HttpMethod.Delete,
                Method.Get => HttpMethod.Get,
                Method.Post => HttpMethod.Post,
                _ => throw new ArgumentOutOfRangeException(nameof(method), method, null)
            };

            var body = JsonConvert.SerializeObject(model); 
            var message = new HttpRequestMessage(httpMethod, resource)
            {
                Content = new JsonContent(body),
            };

            await OnRequestMessageCreatedAsync(message);

            var response = await Client.SendAsync(message, ct);

            await OnRequestMessageSentAsync(response);

            try
            {
                return response.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                await OnRequestExceptionAsync(response, e);
                throw;
            }
        }

        protected virtual Task OnRequestMessageSentAsync(HttpResponseMessage response)
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnRequestMessageCreatedAsync(HttpRequestMessage message)
        {
            return Task.CompletedTask;
        }
        
        protected virtual Task OnRequestExceptionAsync(HttpResponseMessage response, Exception exception)
        {
            return Task.CompletedTask;
        }

    }
}
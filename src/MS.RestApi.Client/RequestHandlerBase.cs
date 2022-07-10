using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MS.RestApi.Client.Exceptions;
using MS.RestApi.Error;
using MS.RestApi.Errors;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MS.RestApi.Client
{
    public class RequestHandlerBase : IRequestHandler
    {
        protected readonly HttpClient Client;

        public RequestHandlerBase(HttpClient client)
        {
            Client = client;
        }
        
        public async Task HandleAsync<TModel>(string resource, TModel model, CancellationToken ct)
        {
            await ExecuteAsync(resource, model, ct);
        }

        public async Task<TResult> HandleAsync<TModel, TResult>(string resource, TModel model, CancellationToken ct)
        {
            var response = await ExecuteAsync(resource, model, ct);
            var responseBody = await response.Content.ReadAsStringAsync();

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

        private async Task<HttpResponseMessage> ExecuteAsync(string resource, object model, CancellationToken ct)
        {
            var body = JsonConvert.SerializeObject(model); 
            var message = new HttpRequestMessage(HttpMethod.Post, resource)
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
            catch (Exception exception)
            {
                await OnRequestExceptionAsync(response, exception);
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var error = default(ApiError);
            var errorType = typeof(GenericApiError);
            var responseObj = JObject.Parse(responseBody);

            if (responseObj.Property(nameof(ApiError.Code)) == null)
            {
                throw new ApiClientException($"Failed to parse API error response: {responseBody.Substring(0, 100)}", response);
            }
            
            if (response.Headers.TryGetValues("X-Error-Type", out var errorTypeHeaders))
            {
                var errorTypeString = errorTypeHeaders.FirstOrDefault();
                
                if (errorTypeString is {Length: > 0})
                {
                    errorType = Type.GetType(errorTypeString!, false) ?? errorType;
                }
            }

            try
            {
                error = (ApiError) responseObj.ToObject(errorType);
            }
            catch (Exception e)
            {
                throw new ApiClientException("Failed to parse API error response.", response, e);
            }

            throw new ApiRemoteErrorException("An unhandled error occured while executing remote request.", error);
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
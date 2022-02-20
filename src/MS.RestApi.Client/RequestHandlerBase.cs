using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MS.RestApi.Abstractions;
using MS.RestApi.Client.Exceptions;
using MS.RestApi.Error;
using MS.RestApi.Error.BadRequest;
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
        
        public async Task HandleAsync<TModel>(Method method, string resource, TModel model, CancellationToken ct)
        {
            await ExecuteAsync(method, resource, model, ct);
        }

        public async Task<TResult> HandleAsync<TModel, TResult>(Method method, string resource, TModel model, CancellationToken ct)
        {
            var response = await ExecuteAsync(method, resource, model, ct);
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
                response.EnsureSuccessStatusCode();
            }
            catch (Exception exception)
            {
                await OnRequestExceptionAsync(response, exception);
                
                throw new ApiRemoteErrorException
                {
                    Error = await BuildApiError(response),
                }; 
            }
            
            return response;
        }

        private async Task<ApiError> BuildApiError(HttpResponseMessage response)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            var responseObj = (JObject)null;
            
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                try
                {
                    responseObj = JObject.Parse(responseBody);
                }
                catch (JsonReaderException)
                {
                    return new ApiError
                    {
                        Code = -2,
                        CodeName = "MalformedResponseError",
                        MessageFormat = "Failed to parse API error response.",
                        LogMessage = $"The response body seems to be not a valid JSON format: {responseBody}",
                    };
                }

                if (responseObj.ContainsKey("code"))
                {
                    if (responseObj.Value<int>("code") == 2)
                    {
                        return responseObj.ToObject<ValidationApiError>();
                    }
                    
                    return responseObj.ToObject<ApiError>();
                }
                
                return new ApiError
                {
                    Code = -3,
                    CodeName = "UnknownResponseError",
                    MessageFormat = "Failed to parse API error response.",
                    LogMessage = $"The response body seems to be not a valid JSON format: {responseBody}",
                };
            }

            return new ApiError
            {
                Code = -1,
                CodeName = "UnknownStatusCode",
                MessageFormat = $"Unknown response code: {(int)response.StatusCode} {response.StatusCode}.",
                LogMessage = $"Bad response received from the API server: {responseBody}",
            };
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
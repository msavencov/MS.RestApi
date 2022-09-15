using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MS.RestApi.Client.Exceptions;
using MS.RestApi.Client.Extensions;
using MS.RestApi.Error;
using MS.RestApi.Errors;
using Newtonsoft.Json;

namespace MS.RestApi.Client;

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
            
            throw new ApiClientException($"Failed to deserialize API response: {responseBody.Truncate(100)}", e);
        }
    }

    private async Task<HttpResponseMessage> ExecuteAsync(string resource, object model, CancellationToken ct)
    {
        var body = JsonConvert.SerializeObject(model); 
        var message = new HttpRequestMessage(HttpMethod.Post, resource)
        {
            Content = new JsonContent(body),
        };

        await OnRequestMessageSendingAsync(message);

        var response = await Client.SendAsync(message, ct);

        await OnRequestMessageSentAsync(message, response);

        try
        {
            return response.EnsureSuccessStatusCode();
        }
        catch (Exception exception)
        {
            await OnRequestExceptionAsync(response, exception);
        }
            
        var responseBody = await response.Content.ReadAsStringAsync() ?? "";
        var error = default(ApiError);
        var errorType = typeof(GenericApiError);
            
        if ((int) response.StatusCode == 555)
        {
            if (response.Headers.TryGetValues("X-Error-Type", out var errorTypeHeaders))
            {
                var errorTypeString = errorTypeHeaders.FirstOrDefault();
                
                if (errorTypeString is {Length: > 0})
                {
                    errorType = Type.GetType(errorTypeString, false) ?? errorType;
                }
            }
            
            try
            {
                error = (ApiError) JsonConvert.DeserializeObject(responseBody, errorType);
            }
            catch (Exception e)
            {
                throw new ApiClientException($"Failed to deserialize API error: {responseBody.Truncate(100)}", e);
            }

            throw new ApiErrorException(error);
        }

        throw new ApiRequestException($"The API response received `{response.StatusCode}: {response.ReasonPhrase}` is not implemented.")
        {
            RequestUrl = message.RequestUri.AbsoluteUri,
            RequestBody = body,
            ResponseCode = (int) response.StatusCode,
            ResponsePhrase = response.ReasonPhrase,
            ResponseBody = responseBody
        };
    }
        
    protected virtual Task OnRequestMessageSentAsync(HttpRequestMessage message, HttpResponseMessage response)
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnRequestMessageSendingAsync(HttpRequestMessage message)
    {
        return Task.CompletedTask;
    }
        
    protected virtual Task OnRequestExceptionAsync(HttpResponseMessage response, Exception exception)
    {
        return Task.CompletedTask;
    }
}
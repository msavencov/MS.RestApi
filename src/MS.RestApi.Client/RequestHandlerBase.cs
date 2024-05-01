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
    
    public async Task HandleAsync<TModel>(IRequestBuilder message, CancellationToken ct = default)
    {
        await ExecuteAsync(message, ct);
    }

    public async Task<TResult> HandleAsync<TModel, TResult>(IRequestBuilder message, CancellationToken ct = default)
    {
        var response = await ExecuteAsync(message, ct);
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

    private async Task<HttpResponseMessage> ExecuteAsync(IRequestBuilder builder, CancellationToken ct)
    {
        var message = builder.GetRequestMessage();
        
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
            Request = message,
            Response = response,
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
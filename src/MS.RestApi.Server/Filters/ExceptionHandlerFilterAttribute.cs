using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MS.RestApi.Error;
using MS.RestApi.Errors;
using MS.RestApi.Server.Exceptions;
using MS.RestApi.Server.Extensions;

namespace MS.RestApi.Server.Filters;

public class ExceptionHandlerFilterAttribute : ExceptionFilterAttribute
{
    private readonly IEnumerable<IServerExceptionHandler> _handlers;

    public ExceptionHandlerFilterAttribute(IEnumerable<IServerExceptionHandler> handlers)
    {
        _handlers = handlers;
    }
    
    public override void OnException(ExceptionContext context)
    {
        if (context.ExceptionHandled)
        {
            return;
        }
            
        var error = default(ApiError);
        var exception = context.Exception;

        if (exception is TargetInvocationException tie)
        {
            exception = tie.InnerException;
        }

        if (exception is AggregateException ae)
        {
            exception = ae.InnerException;
        }

        exception ??= context.Exception;

        if (exception is ApiException apiException)
        {
            error = apiException.ApiError;
        }
            
        if (error == null)
        {
            foreach (var handler in _handlers)
            {
                if (handler.Handle(exception, out error))
                {
                    break;
                }
            }
        }
            
        if (exception is InvalidModelStateException {ModelState: {ErrorCount: > 0} state})
        {
            var detail = state.ToDictionary(t => t.Key, t => t.Value.Errors.Select(e => e.ErrorMessage).ToArray());
            var reason = state.SelectMany(t => t.Value.Errors).Take(3).Select(t => t.ErrorMessage).Join(", ");
            
            error = new ValidationApiError
            {
                Reason = $"Validation errors: {reason}",
                Detail = detail,
                LogMessage = BuildValidationErrorMessage(state),
            };
        }

        error ??= new GenericApiError
        {
            Reason = $"An unhandled error occured: {exception.Message}",
            LogMessage = exception.ToString(),
        };
        
        context.HttpContext.Response.Headers.Add("X-Error-Type", $"{error.GetType().GetFullNameWithAssembly()}");
        context.Result = new ObjectResult(error)
        {
            StatusCode = 555,
        };
        context.ExceptionHandled = true;
    }

    private string BuildValidationErrorMessage(ModelStateDictionary modelState)
    {
        var sb = new StringBuilder().AppendLine("Model validation errors occured.");

        foreach (var item in modelState)
        {
            sb.AppendLine($"Path: {item.Key}");

            foreach (var error in item.Value.Errors)
            {
                sb.AppendLine($"\t{error.ErrorMessage}");
            }
        }

        return sb.ToString();
    }
}
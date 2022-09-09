using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MS.RestApi.Error;
using MS.RestApi.Errors;
using MS.RestApi.Server.Exceptions;

namespace MS.RestApi.Server.Filters
{
    public class ExceptionHandlerFilterAttribute : ExceptionFilterAttribute
    {
        private readonly IEnumerable<IExceptionHandler> _handlers;

        public ExceptionHandlerFilterAttribute(IEnumerable<IExceptionHandler> handlers)
        {
            _handlers = handlers;
        }
        
        public override void OnException(ExceptionContext context)
        {
            if (context.ExceptionHandled || context.Exception == null)
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
                error = apiException.Error;
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
                error = new ValidationApiError
                {
                    Reason = $"Validation errors occurred. See the '{nameof(ValidationApiError.Detail)}' for details.",
                    Detail = context.ModelState.ToDictionary(t => t.Key, t => t.Value.Errors.Select(e => e.ErrorMessage).ToArray()),
                    LogMessage = BuildValidationErrorMessage(state),
                };
            }

            error ??= new GenericApiError
            {
                Reason = $"An unhandled error occured: {exception.Message}",
                LogMessage = exception.ToString(),
            };

            if (context.HttpContext.Response is {HasStarted: false} response)
            {
                response.Headers.Add("X-Error-Type", $"{error.GetType().FullName}, {error.GetType().Assembly.GetName().Name}");
            }
            
            context.Result = new ObjectResult(error)
            {
                StatusCode = (int) HttpStatusCode.InternalServerError,
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
}
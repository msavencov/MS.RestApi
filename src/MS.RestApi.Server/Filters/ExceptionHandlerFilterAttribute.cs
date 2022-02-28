using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MS.RestApi.Error;
using MS.RestApi.Error.BadRequest;
using MS.RestApi.Server.Exceptions;

namespace MS.RestApi.Server.Filters
{
    public class ExceptionHandlerFilterAttribute : ExceptionFilterAttribute
    {
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

            if (exception == null)
            {
                return;
            }

            if (exception is ApiException apiException)
            {
                error = apiException.Error;
            }

            if (exception is InvalidModelStateException {ModelState: {ErrorCount: > 0} state})
            {
                error = new ValidationApiError
                {
                    Code = 2,
                    CodeName = "Validation",
                    ErrorMessage = $"Validation errors occurred. See the 'ValidationErrors' property for details.",
                    ValidationErrors = context.ModelState.ToDictionary(t => t.Key, t => t.Value.Errors.Select(e => e.ErrorMessage).ToArray()),
                    LogMessage = BuildValidationErrorMessage(state),
                };
            }

            if (error is null)
            {
                error = new ApiError()
                {
                    Code = 1,
                    CodeName = "Unhandled",
                    ErrorMessage = $"An unhandled error occured: {exception.Message}",
                    LogMessage = exception.ToString(),
                };
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
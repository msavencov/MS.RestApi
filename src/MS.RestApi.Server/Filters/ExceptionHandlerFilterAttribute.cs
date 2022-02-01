using System;
using System.Linq;
using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MS.RestApi.Error;
using MS.RestApi.Error.BadRequest;

namespace ApiGen.Server.Filters
{
    public class ExceptionHandlerFilterAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.ModelState.IsValid == false)
            {
                var error = new ValidationApiError
                {
                    Code = 400,
                    CodeName = "Validation",
                    MessageFormat = "Validation errors occurred while executing request.",
                    ValidationErrors = context.ModelState.ToDictionary(t => t.Key, t => t.Value.Errors.Select(e => e.ErrorMessage).ToArray())
                };

                context.Result = new ObjectResult(error);
            }
        }

        public override void OnActionExecuted(ActionExecutedContext context)
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

            if (error is null)
            {
                error = new ApiError()
                {
                    Code = 1,
                    CodeName = "Unhandled",
                    MessageFormat = "An unhandled error occured.",
                    LogMessage = exception.ToString(),
                };
            }

            if (context.Result is ObjectResult { Value: ValidationApiError })
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            else
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            
            context.Result = new ObjectResult(error);
            context.ExceptionHandled = true;
        }
    }
}
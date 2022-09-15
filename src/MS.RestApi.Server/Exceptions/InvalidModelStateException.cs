using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MS.RestApi.Server.Exceptions;

internal class InvalidModelStateException : Exception
{
    public ModelStateDictionary ModelState { get; }

    public InvalidModelStateException(ModelStateDictionary modelState)
    {
        ModelState = modelState;
    }
}
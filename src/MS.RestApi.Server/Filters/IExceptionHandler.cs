using System;
using MS.RestApi.Error;

namespace MS.RestApi.Server.Filters;

public interface IServerExceptionHandler
{
    bool Handle(Exception exception, out ApiError error);
}
using System;

namespace MS.RestApi.SourceGenerator.Utils;

internal class ApiGenTypeNotFoundException : ApiGenException
{
    public ApiGenTypeNotFoundException(int id, Type type) : this(id, type.FullName)
    {   
    }
        
    public ApiGenTypeNotFoundException(int id, string type) : base(id, $"The type '{type}' was not found.")
    {
        Category = "ApiGenTypeNotFound";
    }
}
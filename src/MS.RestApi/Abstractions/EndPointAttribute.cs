using System;

namespace MS.RestApi.Abstractions
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EndPointAttribute : Attribute
    {
        public Method Method { get; }
        public string Route { get; }
        public string Group { get; }
        
        public EndPointAttribute(Method method, string route, string group)
        {
            Method = method;
            Route = route;
            Group = group;
        }
    }
}
using System;

namespace MS.RestApi.Abstractions
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EndPointAttribute : Attribute
    {
        public string Route { get; }
        public string Group { get; }
        
        public EndPointAttribute(string route, string group)
        {
            Route = route;
            Group = group;
        }
    }
}
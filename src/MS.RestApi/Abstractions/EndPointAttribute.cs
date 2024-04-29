using System;

namespace MS.RestApi.Abstractions;

[AttributeUsage(AttributeTargets.Class)]
public class EndPointAttribute(string route, string group) : Attribute
{
    public string Route { get; } = route;
    public string Group { get; } = group;
}

[AttributeUsage(AttributeTargets.Class)]
public class SourceAttribute(Source source) : Attribute
{
    public Source Source { get; } = source;
}

public enum Source {Route, Query, Body, Form }


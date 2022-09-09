namespace MS.RestApi.SourceGenerator
{
    [System.AttributeUsage(System.AttributeTargets.Assembly, AllowMultiple = true)]
    internal class ApiGenConfigAttribute : System.Attribute
    {
        public ApiGenConfigAttribute(string key, object value)
        {
            Key = key;
            Value = value;
        }
        
        public string Key { get; }
        public object Value { get; }
    }
}
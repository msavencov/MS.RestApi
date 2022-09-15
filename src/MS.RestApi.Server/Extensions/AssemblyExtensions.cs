using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace MS.RestApi.Server.Extensions;

public static class AssemblyExtensions
{
    public static XDocument GetDocumentation(this Assembly assembly)
    {
        return XDocument.Load(assembly.GetDocumentationFilePath());
    }
        
    public static string GetDocumentationFilePath(this Assembly assembly)
    {
        return Path.ChangeExtension(assembly.Location, "xml");
    }
}
using System.IO;
using System.Reflection;

namespace MS.RestApi.Server.Extensions
{
    public static class AssemblyExtensions
    {
        public static string GetDocumentationFilePath(this Assembly assembly)
        {
            return Path.ChangeExtension(assembly.Location, "xml");
        }
    }
}
using System.IO;
using System.Linq;
using System.Reflection;

namespace MS.RestApi.Generators.Extensions
{
    internal static class ReflectionExtensions
    {
        public static string ReadCodeResource(this Assembly assembly, string resourceName)
        {
            var name = assembly.GetManifestResourceNames().Single(t => t.EndsWith(resourceName));
            var stream = assembly.GetManifestResourceStream(name);

            if (stream is {Length: > 0})
            {
                using (var sr = new StreamReader(stream))
                {
                    return sr.ReadToEnd();
                }
            }

            return null;
        }
    }
}
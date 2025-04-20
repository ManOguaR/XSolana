using System.Linq;

namespace XSolana.Codegen.Tests
{
    // Helper para pascalizar
    internal static class StringExtensions
    {
        public static string ToPascalCase(this string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return string.Concat(s.Split('_').Select(w => char.ToUpperInvariant(w[0]) + w.Substring(1)));
        }
    }
}

using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace XSolana.Builders.Extensions
{
    /// <summary>
    /// Extensions for string manipulation, particularly for transforming strings into PascalCase and resolving C# types.
    /// </summary>
    public static class BuilderStringExtensions
    {
        /// <summary>
        /// Transforms a string into PascalCase, handling snake_case, kebab-case, camelCase, etc.
        /// </summary>
        /// <param name="name">The input string to transform.</param>
        /// <returns>A PascalCase representation of the input.</returns>
        public static string ToPascalCase(this string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return string.Empty;

            // Replace separators with spaces
            name = Regex.Replace(name, @"[_\-\s]+", " ");

            // Insert space before capital letters (camelCase → camel Case)
            name = Regex.Replace(name, @"(?<=[a-z])([A-Z])", " $1");

            // Split and capitalize
            TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;
            var words = name.Split([' '], StringSplitOptions.RemoveEmptyEntries);
            var result = string.Concat(words.Select(w => textInfo.ToTitleCase(w.ToLowerInvariant())));

            return result;
        }

        /// <summary>
        /// Resolves a string type to its corresponding C# type.
        /// </summary>
        /// <param name="type">A string representing a type, which may be in a different format (e.g., "u8", "i32").</param>
        /// <param name="useCodeConvention">A boolean indicating whether to use code conventions for the type resolution.</param>
        /// <param name="useFullName">A boolean indicating whether to use the full name of the type (e.g., "System.String" instead of "string").</param>
        /// <returns>Returns the corresponding C# type as a string.</returns>
        public static string ResolveCSharpType(this string type, bool useCodeConvention = true, bool useFullName = false)
        {
            switch (type)
            {
                case "u8":
                    return useFullName 
                        ? typeof(byte).FullName : useCodeConvention
                        ? "byte" : nameof(Byte);
                case "u16":
                    return useFullName
                        ? typeof(ushort).FullName : useCodeConvention
                        ? "ushort" : nameof(UInt16);
                case "u32":
                    return useFullName
                        ? typeof(uint).FullName : useCodeConvention
                        ? "uint" : nameof(UInt32);
                case "u64":
                    return useFullName
                        ? typeof(ulong).FullName : useCodeConvention
                        ? "ulong" : nameof(UInt64);
                case "i8":
                    return useFullName
                        ? typeof(sbyte).FullName : useCodeConvention
                        ? "sbyte" : nameof(SByte);
                case "i16":
                    return useFullName
                        ? typeof(short).FullName : useCodeConvention
                        ? "short" : nameof(Int16);
                case "i32":
                    return useFullName
                        ? typeof(int).FullName : useCodeConvention
                        ? "int" : nameof(Int32);
                case "i64":
                    return useFullName
                        ? typeof(long).FullName : useCodeConvention
                        ? "long" : nameof(Int64);
                case "bool":
                    return useFullName
                        ? typeof(bool).FullName : useCodeConvention
                        ? "bool" : nameof(Boolean);
                case "string":
                    return useFullName
                        ? typeof(string).FullName : useCodeConvention
                        ? "string" : nameof(String);
                case "publicKey":
                case "pubkey":
                    return useFullName
                        ? typeof(PublicKey).FullName : nameof(PublicKey);
                default:
                    return type;
            }
        }

    }
}

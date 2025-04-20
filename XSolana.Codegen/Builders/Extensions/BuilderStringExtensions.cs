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
        /// Convierte un identificador cualquiera (snake, kebab, pascal, etc.)
        /// en camelCase (minúscula inicial, resto Pascal).
        /// </summary>
        public static string ToCamelCase(this string name)
        {
            var pascal = name.ToPascalCase();
            return string.IsNullOrEmpty(pascal)
                ? string.Empty
                : char.ToLowerInvariant(pascal[0]) + pascal.Substring(1);
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
            return type switch
            {
                "u8" => useFullName
                    ? typeof(byte).FullName : useCodeConvention
                    ? "byte" : nameof(Byte),
                "u16" => useFullName
                    ? typeof(ushort).FullName : useCodeConvention
                    ? "ushort" : nameof(UInt16),
                "u32" => useFullName
                    ? typeof(uint).FullName : useCodeConvention
                    ? "uint" : nameof(UInt32),
                "u64" => useFullName
                    ? typeof(ulong).FullName : useCodeConvention
                    ? "ulong" : nameof(UInt64),
                "i8" => useFullName
                    ? typeof(sbyte).FullName : useCodeConvention
                    ? "sbyte" : nameof(SByte),
                "i16" => useFullName
                    ? typeof(short).FullName : useCodeConvention
                    ? "short" : nameof(Int16),
                "i32" => useFullName
                    ? typeof(int).FullName : useCodeConvention
                    ? "int" : nameof(Int32),
                "i64" => useFullName
                    ? typeof(long).FullName : useCodeConvention
                    ? "long" : nameof(Int64),
                "bool" => useFullName
                    ? typeof(bool).FullName : useCodeConvention
                    ? "bool" : nameof(Boolean),
                "string" => useFullName
                    ? typeof(string).FullName : useCodeConvention
                    ? "string" : nameof(String),
                "publicKey" or "pubkey" => useFullName
                    ? typeof(PublicKey).FullName : nameof(PublicKey),
                _ => type,
            };
        }

        public static int SizeOf(this string type)
        {
            return type switch
            {
                "u8" => 1,
                "i8" => 1,
                "u16" => 2,
                "i16" => 2,
                "u32" => 4,
                "i32" => 4,
                "u64" => 8,
                "i64" => 8,
                _ => throw new NotSupportedException($"Type '{type}' is not supported.")
            };
        }

        //public static bool TryResolveSerializationFunction(this string type, out string functionName)
        //{
        //    functionName = type switch
        //    {
        //        "u8" => "WriteU8",
        //        "i8" => "WriteS8",
        //        "u16" => "WriteU16",
        //        "i16" => "WriteS16",
        //        "u32" => "WriteU32",
        //        "i32" => "WriteS32",
        //        "u64" => "WriteU64",
        //        "i64" => "WriteS64",
        //        "bool" => "WriteBool",
        //        "publicKey" or "pubkey" => "WritePubKey",
        //        _ => ResolveComplexSerializationFunction(type),
        //    };

        //    return !string.IsNullOrEmpty(functionName);
        //}

        //private static string ResolveComplexSerializationFunction(string type)
        //{
        //    if (type.Equals("string", StringComparison.OrdinalIgnoreCase))
        //        return "WriteBorshString"; // long = len U32 + UTF‑8 bytes

        //    if (type.Equals("bytes", StringComparison.OrdinalIgnoreCase) ||
        //        type.Equals("vec<u8>", StringComparison.OrdinalIgnoreCase))
        //        return "WriteVectorU8";  // (*) ver nota más abajo

        //    if (type.StartsWith("vec<", StringComparison.OrdinalIgnoreCase))
        //        return null; // → la plantilla generará un TODO

        //    if (type.StartsWith("option<", StringComparison.OrdinalIgnoreCase))
        //        return null; // → se implementará a mano

        //    return null;
        //}
    }
}

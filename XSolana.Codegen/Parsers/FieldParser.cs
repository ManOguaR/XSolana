using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using XSolana.Conventions;
using XSolana.Parsers.Models;

namespace XSolana.Parsers
{
    /// <summary>
    /// Parses IDL fields from JSON models into field definitions.
    /// </summary>
    public static class FieldParser
    {
        /// <summary>
        /// Parses a list of IDL fields from a List&lt;IdlFieldJsonModel&gt;.
        /// </summary>
        public static List<FieldDefinition> Parse(List<IdlFieldJsonModel> fields)
        {
            var result = new List<FieldDefinition>();

            foreach (var f in fields)
                result.Add(Parse(f));

            return result;
        }

        /// <summary>
        /// Parses a single field JSON model into a field definition.
        /// </summary>
        public static FieldDefinition Parse(IdlFieldJsonModel field)
        {
            return new FieldDefinition
            {
                Name = field.Name,
                Type = ParseType(field.Type)
            };
        }

        /// <summary>
        /// Parses the raw type representation (string or object) into a normalized type string.
        /// </summary>
        private static string ParseType(object typeNode)
        {
            if (typeNode == null)
                return "unknown";

            // Simple string like "u64", "publicKey", "bool"
            if (typeNode is string s)
                return s;

            // If it's a JObject or Dictionary
            if (typeNode is JObject obj)
            {
                var prop = (JProperty)obj.First;
                var typeName = prop.Name;
                var typeValue = prop.Value;

                // Example: { "option": "u64" }
                if (typeValue.Type == JTokenType.String)
                    return $"{typeName}<{typeValue.Value<string>()}>";

                // Example: { "array": ["u8", 32] }
                if (typeValue.Type == JTokenType.Array)
                {
                    var items = typeValue as JArray;
                    return $"{typeName}<{items[0].ToString()}, {items[1].ToString()}>";
                }

                return $"{typeName}<?>";
            }

            return typeNode.ToString();
        }
    }
}

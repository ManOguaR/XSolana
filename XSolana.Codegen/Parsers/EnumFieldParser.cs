using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using XSolana.Conventions;

namespace XSolana.Parsers
{
    /// <summary>
    /// Parses enum fields from JSON models into field definitions.
    /// </summary>
    public static class EnumFieldParser
    {
        /// <summary>
        /// Parses the `fields` object from an enum variant, handling both tuple-like and struct-like formats.
        /// </summary>
        public static List<FieldDefinition> ParseVariantFields(object raw)
        {
            var result = new List<FieldDefinition>();

            if (raw == null)
                return result;

            if (raw is JArray arr)
            {
                foreach (var item in arr)
                {
                    if (item.Type == JTokenType.String)
                    {
                        // tuple-like: ["u64", "publicKey"]
                        result.Add(new FieldDefinition
                        {
                            Name = null, // no name
                            Type = item.ToString()
                        });
                    }
                    else if (item.Type == JTokenType.Object)
                    {
                        var obj = item as JObject;

                        var name = obj["name"]?.ToString();
                        var type = obj["type"];

                        result.Add(new FieldDefinition
                        {
                            Name = name,
                            Type = FieldParserPrivate.ParseType(type)
                        });
                    }
                }
            }

            return result;
        }
    }

    internal static class FieldParserPrivate
    {
        public static string ParseType(JToken typeToken)
        {
            if (typeToken == null)
                return "unknown";

            if (typeToken.Type == JTokenType.String)
                return typeToken.Value<string>();

            if (typeToken.Type == JTokenType.Object)
            {
                var prop = (typeToken as JObject)?.First as JProperty;
                if (prop != null)
                {
                    var typeName = prop.Name;
                    var typeValue = prop.Value;

                    if (typeValue.Type == JTokenType.String)
                        return $"{typeName}<{typeValue.Value<string>()}>";

                    if (typeValue.Type == JTokenType.Array)
                    {
                        var items = typeValue as JArray;
                        return $"{typeName}<{items[0]}, {items[1]}>";
                    }
                }
            }

            return typeToken.ToString();
        }
    }
}

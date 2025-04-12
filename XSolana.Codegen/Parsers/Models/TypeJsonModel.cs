using Newtonsoft.Json;
using System.Collections.Generic;

namespace XSolana.Parsers.Models
{
    /// <summary>
    /// Represents the JSON model for a type in the IDL.
    /// </summary>
    public class TypeJsonModel
    {
        /// <summary>
        /// The name of the type.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// The type of the type.
        /// </summary>
        [JsonProperty("type")]
        public DefinedTypeJsonModel Type { get; set; }
    }

    /// <summary>
    /// Represents the JSON model for a defined type in the IDL.
    /// </summary>
    public class DefinedTypeJsonModel
    {
        /// <summary>
        /// The kind of the type.
        /// </summary>
        [JsonProperty("kind")]
        public string Kind { get; set; } // "struct" o "enum"

        /// <summary>
        /// A list of fields in the type.
        /// </summary>
        [JsonProperty("fields")]
        public List<IdlFieldJsonModel> Fields { get; set; }

        /// <summary>
        /// A list of variants in the type (only for enums).
        /// </summary>
        [JsonProperty("variants")]
        public List<EnumVariantJsonModel> Variants { get; set; }
    }

    /// <summary>
    /// Represents the JSON model for a variant in an enum type.
    /// </summary>
    public class EnumVariantJsonModel
    {
        /// <summary>
        /// The name of the variant.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// A list of fields in the variant.
        /// </summary>
        /// <remarks>
        /// Can be null, an array, or a dictionary (Anchor is inconsistent here).
        /// </remarks>
        [JsonProperty("fields")]
        public object Fields { get; set; }
    }
}

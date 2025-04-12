using Newtonsoft.Json;

namespace XSolana.Parsers.Models
{
    /// <summary>
    /// Represents the JSON model for a field in the IDL.
    /// </summary>
    public class IdlFieldJsonModel
    {
        /// <summary>
        /// The name of the field.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// The type of the field.
        /// </summary>
        /// <remarks>
        /// Can be a string or a complex object.
        /// </remarks>
        [JsonProperty("type")]
        public object Type { get; set; }
    }
}

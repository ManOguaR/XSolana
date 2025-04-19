using Newtonsoft.Json;
using System.Collections.Generic;

namespace XSolana.Parsers.Models
{
    /// <summary>
    /// Represents the JSON model for an event in the IDL.
    /// </summary>
    public class EventJsonModel
    {
        /// <summary>
        /// The name of the event.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// A list of fields in the event.
        /// </summary>
        [JsonProperty("fields")]
        public List<IdlFieldJsonModel> Fields { get; set; } = [];

        /// <summary>
        /// The event's discriminator.
        /// </summary>
        [JsonProperty("discriminator")]
        public List<byte> Discriminator { get; set; }
    }
}

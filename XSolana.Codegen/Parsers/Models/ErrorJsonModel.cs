using Newtonsoft.Json;

namespace XSolana.Parsers.Models
{
    /// <summary>
    /// Represents the error response from the Solana API.
    /// </summary>
    public class ErrorJsonModel
    {
        /// <summary>
        /// The error code.
        /// </summary>
        [JsonProperty("code")]
        public int Code { get; set; }

        /// <summary>
        /// The error name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// The error message.
        /// </summary>
        [JsonProperty("msg")]
        public string Message { get; set; }
    }
}

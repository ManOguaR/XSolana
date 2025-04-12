using Newtonsoft.Json;
using System.Collections.Generic;

namespace XSolana.Parsers.Models
{
    /// <summary>
    /// Represents the JSON model for an account in the IDL.
    /// </summary>
    public class AccountJsonModel
    {
        /// <summary>
        /// The name of the account.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// The type of the account.
        /// </summary>
        [JsonProperty("type")]
        public AccountTypeJsonModel Type { get; set; }
    }

    /// <summary>
    /// Represents the JSON model for the type of an account in the IDL.
    /// </summary>
    public class AccountTypeJsonModel
    {
        /// <summary>
        /// The kind of the account type.
        /// </summary>
        [JsonProperty("kind")]
        public string Kind { get; set; } // Siempre "struct"

        /// <summary>
        /// A list of fields in the account type.
        /// </summary>
        [JsonProperty("fields")]
        public List<IdlFieldJsonModel> Fields { get; set; }
    }
}

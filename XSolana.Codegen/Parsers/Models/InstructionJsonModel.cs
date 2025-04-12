using Newtonsoft.Json;
using System.Collections.Generic;

namespace XSolana.Parsers.Models
{
    /// <summary>
    /// Represents a single instruction in the IDL (Interface Definition Language) JSON model.
    /// </summary>
    public class InstructionJsonModel
    {
        /// <summary>
        /// The name of the instruction.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// The accounts associated with the instruction.
        /// </summary>
        [JsonProperty("accounts")]
        public List<InstructionAccountJsonModel> Accounts { get; set; }

        /// <summary>
        /// The instruction's data type.
        /// </summary>
        [JsonProperty("args")]
        public List<InstructionArgJsonModel> Args { get; set; }
    }

    /// <summary>
    /// Represents an account associated with an instruction in the IDL JSON model.
    /// </summary>
    public class InstructionAccountJsonModel
    {
        /// <summary>
        /// The name of the account.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// The type of the account.
        /// </summary>
        [JsonProperty("isMut")]
        public bool IsMut { get; set; }

        /// <summary>
        /// Indicates whether the account is a signer.
        /// </summary>
        [JsonProperty("isSigner")]
        public bool IsSigner { get; set; }
    }

    /// <summary>
    /// Represents an argument in the instruction's data type in the IDL JSON model.
    /// </summary>
    public class InstructionArgJsonModel
    {
        /// <summary>
        /// The name of the argument.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// The type of the argument.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}

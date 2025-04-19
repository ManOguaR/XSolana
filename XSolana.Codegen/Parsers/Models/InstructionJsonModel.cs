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

        /// <summary>
        /// The instruction's discriminator.
        /// </summary>
        [JsonProperty("discriminator")]
        public List<byte> Discriminator { get; set; }
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
        /// The documentation lines.
        /// </summary>
        [JsonProperty("docs")]
        public List<string> Docs { get; set; }

        /// <summary>
        /// A flag indicating if the account is a writable account.
        /// </summary>
        [JsonProperty("writable")]
        public bool? Writable { get; set; }

        /// <summary>
        /// A flag indicating if the account is a signer.
        /// </summary>
        [JsonProperty("signer")]
        public bool? Signer { get; set; }

        /// <summary>
        /// The PDA (Program Derived Address) associated with the account.
        /// </summary>
        [JsonProperty("pda")]
        public PdaJsonModel Pda { get; set; }

        /// <summary>
        /// The address of the account.
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }
    }

    /// <summary>
    /// Represents the PDA (Program Derived Address) in the IDL JSON model.
    /// </summary>
    public class PdaJsonModel
    {
        /// <summary>
        /// The seeds used to derive the PDA.
        /// </summary>
        [JsonProperty("seeds")]
        public List<PdaSeedJsonModel> Seeds { get; set; }

        /// <summary>
        /// The program associated with the PDA.
        /// </summary>
        [JsonProperty("program")]
        public PdaProgramJsonModel Program { get; set; }
    }

    /// <summary>
    /// Represents a seed used to derive the PDA in the IDL JSON model.
    /// </summary>
    public class PdaSeedJsonModel
    {
        /// <summary>
        /// The seed kind.
        /// </summary>
        [JsonProperty("kind")]
        public string Kind { get; set; }

        /// <summary>
        /// The seed value.
        /// </summary>
        [JsonProperty("value")]
        public object Value { get; set; }  // puede ser array de bytes o string

        /// <summary>
        /// The seed path.
        /// </summary>
        [JsonProperty("path")]
        public string Path { get; set; }

        /// <summary>
        /// The account associated with the seed.
        /// </summary>
        [JsonProperty("account")]
        public string Account { get; set; }
    }

    /// <summary>
    /// Represents the program associated with the PDA in the IDL JSON model.
    /// </summary>
    public class PdaProgramJsonModel
    {
        /// <summary>
        /// The program kind.
        /// </summary>
        [JsonProperty("kind")]
        public string Kind { get; set; }

        /// <summary>
        /// A list of values associated with the program.
        /// </summary>
        [JsonProperty("value")]
        public List<byte> Value { get; set; } = [];
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
        public object Type { get; set; }
    }
}

using Newtonsoft.Json;
using System.Collections.Generic;

namespace XSolana.Parsers.Models
{
    /// <summary>
    /// Represents the IDL (Interface Definition Language) JSON model.
    /// </summary>
    public class IdlJsonModel
    {
        /// <summary>
        /// The address of the program.
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// The metadata associated with the IDL.
        /// </summary>
        [JsonProperty("metadata")]
        public IdlMetadataJsonModel Metadata { get; set; }

        /// <summary>
        /// A list of instructions in the IDL.
        /// </summary>
        [JsonProperty("instructions")]
        public List<InstructionJsonModel> Instructions { get; set; } = [];

        /// <summary>
        /// A list of accounts in the IDL.
        /// </summary>
        [JsonProperty("accounts")]
        public List<AccountJsonModel> Accounts { get; set; } = [];

        /// <summary>
        /// A list of types in the IDL.
        /// </summary>
        [JsonProperty("types")]
        public List<TypeJsonModel> Types { get; set; } = [];

        /// <summary>
        /// A list of events in the IDL.
        /// </summary>
        [JsonProperty("events")]
        public List<EventJsonModel> Events { get; set; } = [];

        /// <summary>
        /// A list of errors in the IDL.
        /// </summary>
        [JsonProperty("errors")]
        public List<ErrorJsonModel> Errors { get; set; } = [];
    }

    /// <summary>
    /// Represents the metadata associated with the IDL.
    /// </summary>
    public class IdlMetadataJsonModel
    {
        /// <summary>
        /// The name of the IDL.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// The version of the IDL.
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; set; }

        /// <summary>
        /// The IDL specification.
        /// </summary>
        [JsonProperty("spec")]
        public string Spec { get; set; }

        /// <summary>
        /// A description of the IDL.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }
    }
}

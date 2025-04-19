using System.Collections.Generic;

namespace XSolana.Conventions
{
    /// <summary>
    /// Represents a program definition in Solana.
    /// </summary>
    public class ProgramDefinition
    {
        /// <summary>
        /// The address (program ID) of the program.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// The name of the program (from metadata).
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The version of the program (from metadata).
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Optional spec version of the IDL.
        /// </summary>
        public string Spec { get; set; }

        /// <summary>
        /// Optional description of the program.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// A list of instructions that the program can handle.
        /// </summary>
        public List<InstructionDefinition> Instructions { get; set; } = [];

        /// <summary>
        /// A list of accounts that the program can handle.
        /// </summary>
        public List<AccountDefinition> Accounts { get; set; } = [];

        /// <summary>
        /// A list of types that the program can handle.
        /// </summary>
        public List<TypeDefinition> Types { get; set; } = [];

        /// <summary>
        /// A list of events that the program can emit.
        /// </summary>
        public List<EventDefinition> Events { get; set; } = [];

        /// <summary>
        /// A list of errors that the program can emit.
        /// </summary>
        public List<ErrorDefinition> Errors { get; set; } = [];
    }
}

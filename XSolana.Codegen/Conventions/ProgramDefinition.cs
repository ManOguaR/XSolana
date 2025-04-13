using System.Collections.Generic;

namespace XSolana.Conventions
{
    /// <summary>
    /// Represents a program definition in Solana.
    /// </summary>
    public class ProgramDefinition
    {
        /// <summary>
        /// The name of the program.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The version of the program.
        /// </summary>
        public string Version { get; set; }

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

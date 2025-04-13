using System.Collections.Generic;

namespace XSolana.Conventions
{
    /// <summary>
    /// Represents the definition of an instruction in a Solana program.
    /// </summary>
    public class InstructionDefinition
    {
        /// <summary>
        /// The name of the instruction.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A list of arguments for the instruction.
        /// </summary>
        public List<FieldDefinition> Args { get; set; } = [];

        /// <summary>
        /// A list of accounts metadata associated with the instruction.
        /// </summary>
        public List<AccountMetaDefinition> Accounts { get; set; } = [];
    }
}

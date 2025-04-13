using System.Collections.Generic;

namespace XSolana.Conventions
{
    /// <summary>
    /// Represents a structure definition in a Solana account or instruction.
    /// </summary>
    public class StructDefinition
    {
        /// <summary>
        /// List of field definitions in the structure.
        /// </summary>
        public List<FieldDefinition> Fields { get; set; } = [];
    }
}

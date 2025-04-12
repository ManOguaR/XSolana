using System.Collections.Generic;

namespace XSolana.Conventions
{
    /// <summary>
    /// Represents a variant of an enum in Solana.
    /// </summary>
    public class EnumVariant
    {
        /// <summary>
        /// The name of the enum variant.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A list of field definitions associated with the enum variant.
        /// </summary>
        /// <remarks>Null si es un enum sin datos (unit variant).</remarks>
        public List<FieldDefinition> Fields { get; set; }
    }
}
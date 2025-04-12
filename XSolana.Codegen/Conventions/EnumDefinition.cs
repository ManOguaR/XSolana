using System.Collections.Generic;

namespace XSolana.Conventions
{
    /// <summary>
    /// Represents an enum definition in Solana.
    /// </summary>
    public class EnumDefinition
    {
        /// <summary>
        /// A list of enum variants.
        /// </summary>
        public List<EnumVariant> Variants { get; set; } = new List<EnumVariant>();
    }

}
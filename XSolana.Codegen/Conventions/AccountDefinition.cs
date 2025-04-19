using System.Collections.Generic;

namespace XSolana.Conventions
{
    /// <summary>
    /// Represents an account definition in Solana.
    /// </summary>
    public class AccountDefinition
    {
        /// <summary>
        /// The name of the account.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The account's structure definition.
        /// </summary>
        public StructDefinition Type { get; set; }

        /// <summary>
        /// A list of discriminators for the account.
        /// </summary>
        public List<byte> Discriminator { get; set; } = [];

    }
}

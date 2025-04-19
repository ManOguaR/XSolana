using System.Collections.Generic;

namespace XSolana.Conventions
{
    /// <summary>
    /// Represents a metadata definition for an account in Solana.
    /// </summary>
    public class AccountMetaDefinition
    {
        /// <summary>
        /// The name of the account.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Indicates whether the account is writable.
        /// </summary>
        public bool IsMut { get; set; }

        /// <summary>
        /// Indicates whether the account is a signer.
        /// </summary>
        public bool IsSigner { get; set; }
        public string Address { get; internal set; }
        public List<string> Docs { get; internal set; }
        public PdaDefinition Pda { get; internal set; }
    }
}

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
    }
}

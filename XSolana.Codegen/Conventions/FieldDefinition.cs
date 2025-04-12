namespace XSolana.Conventions
{
    /// <summary>
    /// Represents a field definition in a Solana account or instruction.
    /// </summary>
    public class FieldDefinition
    {
        /// <summary>
        /// The name of the field.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The type of the field, which can be a primitive type or a complex type.
        /// </summary>
        /// <remarks>It can be u64, vec, option, etc.</remarks>
        public string Type { get; set; }
    }
}

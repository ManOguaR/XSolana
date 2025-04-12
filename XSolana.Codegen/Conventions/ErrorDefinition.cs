namespace XSolana.Conventions
{
    /// <summary>
    /// Represents an error definition.
    /// </summary>
    public class ErrorDefinition
    {
        /// <summary>
        /// The error code.
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// The error name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The error message.
        /// </summary>
        public string Message { get; set; }
    }
}

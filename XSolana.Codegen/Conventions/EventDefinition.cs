using System.Collections.Generic;

namespace XSolana.Conventions
{
    /// <summary>
    /// Represents an event definition in a Solana program.
    /// </summary>
    public class EventDefinition
    {
        /// <summary>
        /// The name of the event.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A list of field definitions that describe the data associated with the event.
        /// </summary>
        public List<FieldDefinition> Fields { get; set; } = new List<FieldDefinition>();
    }
}

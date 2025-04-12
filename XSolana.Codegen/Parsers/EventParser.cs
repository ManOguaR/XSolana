using System.Collections.Generic;
using XSolana.Conventions;
using XSolana.Parsers.Models;

namespace XSolana.Parsers
{
    /// <summary>
    /// Parses event JSON models into event definitions.
    /// </summary>
    public static class EventParser
    {
        /// <summary>
        /// Parses a list of event JSON models into event definitions.
        /// </summary>
        public static List<EventDefinition> Parse(List<EventJsonModel> source)
        {
            var result = new List<EventDefinition>();

            foreach (var evt in source)
                result.Add(Parse(evt));

            return result;
        }

        /// <summary>
        /// Parses a single event JSON model into an event definition.
        /// </summary>
        public static EventDefinition Parse(EventJsonModel model)
        {
            var fields = FieldParser.Parse(model.Fields);

            return new EventDefinition
            {
                Name = model.Name,
                Fields = fields
            };
        }
    }
}

using System.Collections.Generic;
using XSolana.Conventions;
using XSolana.Parsers.Models;

namespace XSolana.Parsers
{
    /// <summary>
    /// Parses error JSON models into error definitions.
    /// </summary>
    public static class ErrorParser
    {
        /// <summary>
        /// Parses a list of error JSON models into error definitions.
        /// </summary>
        public static List<ErrorDefinition> Parse(List<ErrorJsonModel> source)
        {
            var result = new List<ErrorDefinition>();

            foreach (var err in source)
                result.Add(Parse(err));

            return result;
        }

        /// <summary>
        /// Parses a single error JSON model into an error definition.
        /// </summary>
        public static ErrorDefinition Parse(ErrorJsonModel model)
        {
            return new ErrorDefinition
            {
                Code = model.Code,
                Name = model.Name,
                Message = model.Message
            };
        }
    }
}

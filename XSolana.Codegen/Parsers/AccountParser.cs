using System.Collections.Generic;
using XSolana.Conventions;
using XSolana.Parsers.Models;

namespace XSolana.Parsers
{
    /// <summary>
    /// Parses account JSON models into account definitions.
    /// </summary>
    public static class AccountParser
    {
        /// <summary>
        /// Parses a list of account JSON models into account definitions.
        /// </summary>
        public static List<AccountDefinition> Parse(List<AccountJsonModel> source)
        {
            var result = new List<AccountDefinition>();

            foreach (var acc in source)
                result.Add(Parse(acc));

            return result;
        }

        /// <summary>
        /// Parses a single account JSON model into an account definition.
        /// </summary>
        public static AccountDefinition Parse(AccountJsonModel model)
        {
            var fields = model.Type?.Fields != null
                    ? FieldParser.Parse(model.Type.Fields)
                    : [];

            return new AccountDefinition
            {
                Name = model.Name,
                Discriminator = model.Discriminator,
                Type = new StructDefinition
                {
                    Fields = fields
                }
            };
        }
    }
}


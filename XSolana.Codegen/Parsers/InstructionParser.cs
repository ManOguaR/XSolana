using System.Collections.Generic;
using XSolana.Conventions;
using XSolana.Parsers.Models;

namespace XSolana.Parsers
{
    /// <summary>
    /// Parses instruction JSON models into instruction definitions.
    /// </summary>
    public static class InstructionParser
    {
        /// <summary>
        /// Parses a list of instruction JSON models into instruction definitions.
        /// </summary>
        public static List<InstructionDefinition> Parse(List<InstructionJsonModel> source)
        {
            var result = new List<InstructionDefinition>();

            foreach (var instr in source)
                result.Add(Parse(instr));

            return result;
        }

        /// <summary>
        /// Parses a single instruction JSON model into an instruction definition.
        /// </summary>
        public static InstructionDefinition Parse(InstructionJsonModel instr)
        {
            var args = new List<FieldDefinition>();
            foreach (var arg in instr.Args)
            {
                args.Add(new FieldDefinition
                {
                    Name = arg.Name,
                    Type = arg.Type
                });
            }

            var accounts = new List<AccountMetaDefinition>();
            foreach (var acc in instr.Accounts)
            {
                accounts.Add(new AccountMetaDefinition
                {
                    Name = acc.Name,
                    IsMut = acc.IsMut,
                    IsSigner = acc.IsSigner
                });
            }

            return new InstructionDefinition
            {
                Name = instr.Name,
                Args = args,
                Accounts = accounts
            };
        }
    }
}

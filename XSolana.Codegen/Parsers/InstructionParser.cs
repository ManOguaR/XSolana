using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                    Type = FieldParser.ParseType(arg.Type)
                });
            }

            var accounts = new List<AccountMetaDefinition>();
            foreach (var acc in instr.Accounts)
            {
                accounts.Add(new AccountMetaDefinition
                {
                    Name = acc.Name,
                    IsMut = acc.Writable ?? false,
                    IsSigner = acc.Signer ?? false,
                    Address = acc.Address,
                    Docs = acc.Docs,
                    Pda = acc.Pda != null
                        ? new PdaDefinition
                        {
                            Seeds = ConvertSeeds(acc.Pda.Seeds),
                            Program = acc.Pda.Program != null
                                ? new PdaProgramDefinition
                                {
                                    Kind = acc.Pda.Program.Kind,
                                    Value = acc.Pda.Program.Value   // ← ya es List<byte>
                                }
                                : null
                        }
                        : null
                });
            }

            return new InstructionDefinition
            {
                Name = instr.Name,
                Args = args,
                Accounts = accounts,
                Discriminator = instr.Discriminator
            };

        }
        private static List<PdaSeedDefinition> ConvertSeeds(List<PdaSeedJsonModel> src)
        {
            if (src == null || src.Count == 0) return null;

            var list = new List<PdaSeedDefinition>(src.Count);

            foreach (var s in src)
            {
                var dst = new PdaSeedDefinition();

                switch (s.Kind.ToLowerInvariant())
                {
                    /*-------------------- CONST --------------------*/
                    case "const":
                        dst.Kind = PdaSeedKind.Const;

                        // El valor puede venir como JArray, List<object> o string
                        switch (s.Value)
                        {
                            case JArray ja:                     // "[1,2,3]"
                                dst.ConstBytes = [.. ja.Values<byte>()];
                                break;

                            case IList<object> ol:              // ya deserializado en una List<object>
                                dst.ConstBytes = [.. ol.Select(o => Convert.ToByte(o))];
                                break;

                            case string str:                    // "ascii-string"
                                dst.ConstBytes = Encoding.ASCII.GetBytes(str);
                                break;

                            default:
                                throw new InvalidOperationException(
                                    $"Seed.const con valor inesperado ({s.Value?.GetType().Name ?? "null"})");
                        }
                        break;

                    /*------------------- ACCOUNT -------------------*/
                    case "account":
                        dst.Kind = PdaSeedKind.Account;
                        dst.Path = s.Path;
                        dst.Account = s.Account;
                        break;

                    /*--------------------- ARG ---------------------*/
                    case "arg":
                        dst.Kind = PdaSeedKind.Arg;
                        dst.Path = s.Path;
                        break;

                    default:
                        throw new NotSupportedException($"Seed kind '{s.Kind}' no soportado.");
                }

                list.Add(dst);
            }

            return list;
        }
    }
}

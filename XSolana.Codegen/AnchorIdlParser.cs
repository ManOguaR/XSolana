using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using XSolana.Conventions;
using XSolana.Parsers;
using XSolana.Parsers.Models;

namespace XSolana
{
    /// <summary>
    /// Parses an Anchor IDL (Interface Definition Language) file into a program definition.
    /// </summary>
    public class AnchorIdlParser
    {
        /// <summary>
        /// Parses an Anchor IDL file from the specified path and returns a ProgramDefinition object.
        /// </summary>
        /// <param name="path">The path to the IDL file.</param>
        /// <returns>The parsed ProgramDefinition object.</returns>
        public ProgramDefinition ParseFromFile(string path)
        {
            var json = File.ReadAllText(path);
            return ParseFromJson(json);
        }

        /// <summary>
        /// Parses an Anchor IDL JSON string into a ProgramDefinition object.
        /// </summary>
        /// <param name="json">The JSON string representing the IDL.</param>
        /// <returns>A ProgramDefinition object representing the parsed IDL.</returns>
        /// <exception cref="InvalidDataException">Thrown when the IDL cannot be deserialized.</exception>
        public ProgramDefinition ParseFromJson(string json)
        {
            var idl = JsonConvert.DeserializeObject<IdlJsonModel>(json) ?? throw new InvalidDataException("El IDL no pudo deserializarse.");
            var program = new ProgramDefinition
            {
                Address = idl.Address,
                Name = idl.Metadata?.Name,
                Version = idl.Metadata?.Version,
                Spec = idl.Metadata?.Spec,
                Description = idl.Metadata?.Description,
                Instructions = InstructionParser.Parse(idl.Instructions ?? []),
                Accounts = AccountParser.Parse(idl.Accounts ?? []),
                Types = TypeParser.Parse(idl.Types ?? []),
                Events = EventParser.Parse(idl.Events ?? []),
                Errors = ErrorParser.Parse(idl.Errors ?? [])
            };

            return program;
        }
    }
}

using System;
using System.Linq;
using XSolana.Builders.Extensions;
using XSolana.Conventions;

namespace XSolana.Builders
{
    public sealed class EventsBuilder : CodeBuilderBase
    {
        public EventsBuilder(string className, string @namespace)
            : base(className, @namespace)
        {
            Includes = new[]
            {
                "System",
                "System.Collections.Generic",
                "System.Buffers.Binary",
                "Solnet.Wallet",
                "Solnet.Programs.Utilities"
            };
        }

        public override string TransformText(ProgramDefinition program)
        {
            string evtNamespace = $"Generated.{program.Name}";
            WriteFileHeader();
            BeginNamespace();

            // ------ static hub -------------------------------------------------
            string hubName = program.Name.ToPascalCase() + "Events";
            BeginClass(hubName, isStatic: true);

            //BeginClass(null, isStatic: true, name: $"static partial"); // trick ignore, just to keep format?

            // dictionary
            WriteLine("private static readonly Dictionary<ulong, Func<byte[], object>> _parsers = new();");
            WriteReturn();

            // static ctor
            WriteLine("static " + hubName + "()");
            BeginBlock();
            foreach (var ev in program.Events)
            {
                ulong disc = BitConverter.ToUInt64([.. ev.Discriminator], 0);
                WriteLine($"_parsers[{disc}UL] = data => {ev.Name.ToPascalCase()}Event.TryDeserialize(data, out var e) ? e : null;");
            }
            EndBlock();
            WriteReturn();

            // generic TryParse
            WriteLine("public static bool TryParse(byte[] data, out object evt)");
            BeginBlock();
            WriteLine("evt = null;");
            WriteLine("if (data.Length < 8) return false;");
            WriteLine("ulong disc = BinaryPrimitives.ReadUInt64LittleEndian(data);");
            WriteLine("return _parsers.TryGetValue(disc, out var p) && (evt = p(data[8..])) != null;"); EndBlock();
            EndType(); // hub
            WriteReturn();

            // ------ one record per event --------------------------------------
            foreach (var ev in program.Events)
                EmitEventClass(ev);

            EndNamespace();
            return Code.ToString();
        }

        /* ------------------------------------------------------------- */
        private void EmitEventClass(EventDefinition ev)
        {
            string className = ev.Name.ToPascalCase() + "Event";
            string ctorParams = string.Join(", ",
                ev.Fields.Select(f => $"{f.Type.ResolveCSharpType()} {f.Name.ToCamelCase()}"));
            string props = string.Join(", ",
                ev.Fields.Select(f => f.Name.ToPascalCase()));

            // record
            BeginClass(className);

            foreach (var f in ev.Fields)
            {
                string propType = f.Type.ResolveCSharpType();
                string propName = f.Name.ToPascalCase();
                WriteLine($"public {propType} {propName} {{ get; }}");
                WriteReturn();
            }

            WriteLine($"public {className}({ctorParams})");
            BeginBlock();
            foreach (var f in ev.Fields)
            {
                string paramName = f.Name.ToCamelCase();
                string propName = f.Name.ToPascalCase();
                WriteLine($"this.{propName} = {paramName};");
            }
            EndBlock();
            WriteReturn();

            // TryDeserialize
            WriteLine($"public static bool TryDeserialize(byte[] data, out {className} e)");
            BeginBlock();
            if (ev.Fields.Count > 0)
            {
                WriteLine("var span = data.AsSpan();");
                WriteLine("int offset = 0;");
                foreach (var f in ev.Fields)
                {
                    string tmp = f.Name.ToCamelCase();
                    EmitReadCode(f.Type, tmp);
                }
            }
            string ctorArgs = string.Join(", ", ev.Fields.Select(f => f.Name.ToCamelCase()));
            WriteLine($"e = new {className}({ctorArgs});");
            WriteLine("return true;");
            EndBlock();
            EndType();
            WriteReturn();

            // helper read generator (local)
            void EmitReadCode(string idlType, string target)
            {
                idlType = idlType.ToLowerInvariant();
                switch (idlType)
                {
                    case "pubkey" or "publickey":
                        WriteLine($"var {target} = span.GetPubKey(offset);");
                        WriteLine("offset += 32;"); return;
                    case "u64": WriteLine($"var {target} = span.GetU64(offset);"); WriteLine("offset += 8;"); return;
                    case "i64": WriteLine($"var {target} = span.GetS64(offset);"); WriteLine("offset += 8;"); return;
                    case "u32": WriteLine($"var {target} = span.GetU32(offset);"); WriteLine("offset += 4;"); return;
                    case "i32": WriteLine($"var {target} = span.GetS32(offset);"); WriteLine("offset += 4;"); return;
                    case "u16": WriteLine($"var {target} = span.GetU16(offset);"); WriteLine("offset += 2;"); return;
                    case "i16": WriteLine($"var {target} = span.GetS16(offset);"); WriteLine("offset += 2;"); return;
                    case "u8": WriteLine($"var {target} = span.GetU8(offset);"); WriteLine("offset += 1;"); return;
                    case "i8": WriteLine($"var {target} = span.GetS8(offset);"); WriteLine("offset += 1;"); return;
                    case "bool": WriteLine($"var {target} = span.GetBool(offset);"); WriteLine("offset += 1;"); return;
                    default:
                        WriteLine($"// TODO: leer campo '{target}' ({idlType})");
                        WriteLine("offset += 0;");
                        return;
                }
            }
        }
    }
}

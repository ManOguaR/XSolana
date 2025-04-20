using System.Collections.Generic;
using System.Linq;
using XSolana.Builders.Extensions;
using XSolana.Conventions;

namespace XSolana.Builders
{
    /// <summary>
    /// Genera structs con los argumentos y un método Encode(byte[]) por instrucción.
    /// </summary>
    public class InstructionDataBuilder : CodeBuilderBase
    {
        public InstructionDataBuilder(string className, string @namespace)
            : base(className, @namespace)
        {
            Includes =
            [
                "System",
                "System.Collections.Generic",
                "Solnet.Wallet",
                "Solnet.Programs.Utilities"
            ];
        }

        public override string TransformText(ProgramDefinition program)
        {
            WriteFileHeader();
            BeginNamespace();

            foreach (var instr in program.Instructions)
            {
                var (sizeLines, writeLines) = BuildLines(instr.Args);

                if(instr.Args.Count > 0)
                {
                    // Struct con propiedades
                    BeginStruct(instr.Name.ToPascalCase() + "Args");
                    foreach (var arg in instr.Args)
                        WriteLine($"public {arg.Type.ResolveCSharpType()} {arg.Name.ToPascalCase()} {{ get; set; }}");
                    EndType();
                    WriteReturn();
                }

                // Clase estática con método Encode(...)
                BeginClass(instr.Name.ToPascalCase() + "Data", isStatic: true);

                // firmas de los parámetros
                var paramList = string.Join(", ",
                    instr.Args.Select(a => $"{a.Type.ResolveCSharpType()} {a.Name.ToCamelCase()}"));

                WriteLine($"public static byte[] Encode({paramList})");
                BeginBlock();

                // discriminador (array literal)
                var discLiteral = string.Join(", ", instr.Discriminator);
                WriteLine($"var discriminator = new byte[] {{ {discLiteral} }};");
                WriteReturn();
                WriteLine("int size = discriminator.Length;");
                foreach (var l in sizeLines) WriteLine(l);
                WriteReturn();
                // payload
                WriteLine("byte[] data = new byte[size];");
                WriteLine("int offset = 0;");
                WriteLine("data.WriteSpan(discriminator, offset);");
                WriteLine("offset += discriminator.Length;");
                foreach (var l in writeLines) WriteLine(l);
                WriteReturn();
                WriteLine("return data;");

                EndBlock();   // Encode
                EndType();    // class Data
                WriteReturn();
            }

            EndNamespace();
            return Code.ToString();
        }

        private (List<string> sizeLines, List<string> writeLines) BuildLines(List<FieldDefinition> args)
        {
            var sizeLines = new List<string>();
            var writeLines = new List<string>();

            foreach (var arg in args)
            {
                var (sz, wr) = BuildLines(arg.Type, arg.Name.ToCamelCase());
                sizeLines.AddRange(sz);
                writeLines.AddRange(wr);
            }


            return (sizeLines, writeLines);
        }

        private (List<string> sizeLines, List<string> writeLines) BuildLines(string idlType, string varName)
        {
            var sz = new List<string>();
            var wr = new List<string>();

            switch (idlType.ToLowerInvariant())
            {
                // ---------- primitivos ----------
                case "u64": Add("WriteU64", idlType.ResolveCSharpType(), varName); break;
                case "i64": Add("WriteS64", idlType.ResolveCSharpType(), varName); break;
                case "u32": Add("WriteU32", idlType.ResolveCSharpType(), varName); break;
                case "i32": Add("WriteS32", idlType.ResolveCSharpType(), varName); break;
                case "u16": Add("WriteU16", idlType.ResolveCSharpType(), varName); break;
                case "i16": Add("WriteS16", idlType.ResolveCSharpType(), varName); break;
                case "u8": Add("WriteU8", idlType.ResolveCSharpType(), varName); break;
                case "i8": Add("WriteS8", idlType.ResolveCSharpType(), varName); break;
                case "bool": Add("WriteBool", idlType.ResolveCSharpType(), varName); break;

                // ---------- pubkey (32) ----------
                case "pubkey":
                case "publickey":
                    AddFixed("WritePubKey", 32, varName);
                    break;

                // ---------- string ----------
                case "string":
                    sz.Add($"size += 4 + System.Text.Encoding.UTF8.GetByteCount({varName});");     // u32 len + bytes
                    wr.Add($"offset += data.WriteBorshString({varName}, offset);");
                    break;

                // ---------- bytes / vec<u8> ----------
                case "bytes":
                case "vec<u8>":
                    sz.Add($"size += 4 + {varName}.Length;");
                    wr.Add($"data.WriteU32((uint){varName}.Length, offset);");
                    wr.Add("offset += 4;");
                    wr.Add($"data.WriteSpan({varName}, offset);");
                    wr.Add($"offset += {varName}.Length;");
                    break;

                default:
                    {
                        // ---------- vec<T> ----------
                        if (idlType.StartsWith("vec<"))
                        {
                            string inner = idlType.Substring(4, idlType.Length - 5);
                            sz.Add($"size += 4;\t\t// u32 len"); // u32 len
                            sz.Add($"foreach(var e in {varName})");
                            sz.Add("{");
                            var (iSz, iWr) = BuildLines(inner, "e");
                            sz.AddRange(iSz.Select(l => "\t" + l));
                            sz.Add("}");
                            wr.Add($"data.WriteU32((uint){varName}.Count, offset);");
                            wr.Add("offset += 4;\t\t// u32 len");
                            wr.Add($"foreach(var e in {varName})");
                            wr.Add("{");
                            wr.AddRange(iWr.Select(l => "\t" + l));
                            wr.Add("}");
                            break;

                        }
                        else if (idlType.StartsWith("option<"))
                        {
                            string inner = idlType.Substring(7, idlType.Length - 8);
                            sz.Add("size += 1;\t\t// flag"); // flag
                            sz.Add($"if({varName}.HasValue)");
                            sz.Add("{");
                            var (iSz, iWr) = BuildLines(inner, $"{varName}.Value");
                            sz.AddRange(iSz.Select(l => "\t" + l));
                            sz.Add("}");
                            wr.Add($"data.WriteBool({varName}.HasValue, offset);");
                            wr.Add("offset += 1;\t\t// flag");
                            wr.Add($"if({varName}.HasValue)");
                            wr.Add("{");
                            wr.AddRange(iWr.Select(l => "\t" + l));
                            wr.Add("}");
                            break;
                        }
                        else
                        {
                            sz.Add($"size += {varName}.GetSize();");
                            wr.Add($"offset += {varName}.Serialize(data, offset);");
                            break;
                        }
                    }
            }
            return (sz, wr);

            // ---------- local helpers ----------
            void Add(string fn, string type, string name = null)
            {
                sz.Add($"size += sizeof({type});{(string.IsNullOrEmpty(name) ? "" : $"\t\t// {name}")}");
                wr.Add($"data.{fn}({varName}, offset);");
                wr.Add($"offset += sizeof({type});");
            }

            void AddFixed(string fn, int bytes, string name = null)
            {
                sz.Add($"size += {bytes};{(string.IsNullOrEmpty(name) ? "" : $"\t\t// {name}")}");
                wr.Add($"data.{fn}({varName}, offset);");
                wr.Add($"offset += {bytes};");
            }
        }

        //private void EmitSerializeCode(string idlType, string varName)
        //{
        //    idlType = idlType.Trim().ToLowerInvariant();

        //    // PRIMITIVOS --------------------------------------------------------
        //    if (idlType is "u64" or "i64" or "u32" or "i32" or "u16" or "i16"
        //                   or "u8" or "i8" or "bool")
        //    {
        //        string fn = idlType switch
        //        {
        //            "u8" => "WriteU8",
        //            "i8" => "WriteS8",
        //            "u16" => "WriteU16",
        //            "i16" => "WriteS16",
        //            "u32" => "WriteU32",
        //            "i32" => "WriteS32",
        //            "u64" => "WriteU64",
        //            "i64" => "WriteS64",
        //            "bool" => "WriteBool",
        //            _ => string.Empty
        //        };
        //        if (string.IsNullOrEmpty(fn))
        //            throw new NotSupportedException($"Type '{idlType}' is not supported.");
        //        WriteLine($"data.{fn}({varName}, offset);");
        //        WriteLine($"offset += {(idlType.SizeOf())};");
        //        return;
        //    }

        //    // PUBKEY ------------------------------------------------------------
        //    if (idlType is "pubkey" or "publickey")
        //    {
        //        WriteLine($"data.WritePubKey({varName}, offset);");
        //        WriteLine("offset += 32;");
        //        return;
        //    }

        //    // STRING ------------------------------------------------------------
        //    if (idlType == "string")
        //    {
        //        WriteLine($"data.WriteBorshString({varName}, ref offset);");
        //        return;
        //    }

        //    // BYTES / VEC<U8> ---------------------------------------------------
        //    if (idlType == "bytes" || idlType == "vec<u8>")
        //    {
        //        WriteLine($"data.WriteU32((uint){varName}.Length, offset);");
        //        WriteLine("offset += 4;");
        //        WriteLine($"data.WriteSpan({varName}, offset);");
        //        WriteLine($"offset += {varName}.Length;");
        //        return;
        //    }

        //    // VEC<T> ------------------------------------------------------------
        //    if (idlType.StartsWith("vec<"))
        //    {
        //        string inner = idlType.Substring(4, idlType.Length - 5);
        //        WriteLine($"data.WriteU32((uint){varName}.Count, offset);");
        //        WriteLine("offset += 4;");
        //        WriteLine($"foreach(var e in {varName})");
        //        BeginBlock();
        //        EmitSerializeCode(inner, "e");      // recursivo
        //        EndBlock();
        //        return;
        //    }

        //    // OPTION<T> ---------------------------------------------------------
        //    if (idlType.StartsWith("option<"))
        //    {
        //        string inner = idlType.Substring(7, idlType.Length - 8);
        //        WriteLine($"data.WriteBool({varName}.HasValue, offset);");
        //        WriteLine("offset += 1;");
        //        WriteLine($"if({varName}.HasValue)");
        //        BeginBlock();
        //        EmitSerializeCode(inner, $"{varName}.Value");
        //        EndBlock();
        //        return;
        //    }

        //    // STRUCT / ENUM anidado --------------------------------------------
        //    // asumimos que genera un método Serialize(...)
        //    WriteLine($"{varName}.Serialize(data, ref offset);");
        //}

    }
}



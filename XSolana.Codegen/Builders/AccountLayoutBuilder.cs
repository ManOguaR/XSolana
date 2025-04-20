using System;
using System.Linq;
using XSolana.Builders.Extensions;
using XSolana.Conventions;

namespace XSolana.Builders
{
    /// <summary>
    /// Genera un layout C# por cada cuenta definida en el IDL
    /// e incluye un método <c>TryDeserialize</c> compatible con Solnet.
    /// </summary>
    public class AccountLayoutBuilder : CodeBuilderBase
    {
        public AccountLayoutBuilder(string className, string @namespace)
            : base(className, @namespace)
        {
            Includes = new[]
            {
                "System",
                "System.Collections.Generic",
                "Solnet.Wallet",
                "Solnet.Programs.Utilities"
            };
        }

        //------------------------------------------------------------------
        // ENTRY POINT
        //------------------------------------------------------------------
        public override string TransformText(ProgramDefinition program)
        {
            WriteFileHeader();
            BeginNamespace();

            foreach (var acc in program.Accounts)
            {
                // ‑‑ buscamos la definición de campos en program.Types
                // 1. obtenemos la lista de campos del struct
                var structDef = acc.Type?.Fields;

                if (structDef == null || structDef.Count == 0)
                {
                    structDef = program.Types
                        .FirstOrDefault(t =>
                            t.Kind == TypeKind.Struct &&
                            string.Equals(t.Name, acc.Name, StringComparison.OrdinalIgnoreCase))
                        ?.Struct?.Fields
                        ?? throw new InvalidOperationException(
                               $"No se encontró struct '{acc.Name}' en types[].");
                }

                BeginClass(acc.Name.ToPascalCase(), isStatic: false);

                //------------------------------------------------------------------
                // 1) Constantes de discriminador
                //------------------------------------------------------------------
                var discLiteral = string.Join(", ", acc.Discriminator);
                WriteLine($"public static readonly byte[] Discriminator = new byte[] {{ {discLiteral} }};");
                WriteReturn();

                //------------------------------------------------------------------
                // 2) Propiedades
                //------------------------------------------------------------------
                foreach (var field in structDef)
                    WriteLine($"public {field.Type.ResolveCSharpType()} {field.Name.ToPascalCase()} {{ get; set; }}");

                WriteReturn();

                //------------------------------------------------------------------
                // 3) TryDeserialize(ReadOnlySpan<byte>)
                //------------------------------------------------------------------
                WriteLine("public static bool TryDeserialize(byte[] source, out "
                          + $"{acc.Name.ToPascalCase()} account)");
                BeginBlock();
                WriteLine("account = null;");
                WriteLine("if(source.Length < Discriminator.Length) return false;");
                WriteLine("var data = new ReadOnlySpan<byte>(source);");
                WriteLine("if(!data.Slice(0,8).SequenceEqual(Discriminator)) return false;");
                WriteLine("int offset = 8;");
                WriteLine($"var result = new {acc.Name.ToPascalCase()}();");

                foreach (var f in structDef)
                {
                    string propExpr = "result." + f.Name.ToPascalCase();
                    EmitReadCode(f.Type, propExpr);
                }

                WriteLine("account = result;");
                WriteLine("return true;");
                EndBlock();          // TryDeserialize
                EndType();           // class Account
                WriteReturn();
            }

            EndNamespace();
            return Code.ToString();
        }

        private void EmitReadCode(string idlType, string target)
        {
            idlType = idlType.Trim().ToLowerInvariant();

            // ---------- PRIMITIVOS ------------------------------------------
            switch (idlType)
            {
                case "u64": WriteLine($"{target} = data.GetU64(offset);"); WriteLine("offset += 8;"); return;
                case "i64": WriteLine($"{target} = data.GetS64(offset);"); WriteLine("offset += 8;"); return;
                case "u32": WriteLine($"{target} = data.GetU32(offset);"); WriteLine("offset += 4;"); return;
                case "i32": WriteLine($"{target} = data.GetS32(offset);"); WriteLine("offset += 4;"); return;
                case "u16": WriteLine($"{target} = data.GetU16(offset);"); WriteLine("offset += 2;"); return;
                case "i16": WriteLine($"{target} = data.GetS16(offset);"); WriteLine("offset += 2;"); return;
                case "u8": WriteLine($"{target} = data.GetU8(offset);"); WriteLine("offset += 1;"); return;
                case "i8": WriteLine($"{target} = data.GetS8(offset);"); WriteLine("offset += 1;"); return;
                case "bool": WriteLine($"{target} = data.GetBool(offset);"); WriteLine("offset += 1;"); return;
                case "pubkey" or "publickey":
                    WriteLine($"{target} = data.GetPubKey(offset);");
                    WriteLine("offset += 32;");
                    return;
                default:
                    {
                        // ---------- STRING ----------------------------------------------
                        if (idlType == "string")
                        {
                            WriteLine("{");
                            IndentAdd();
                            WriteLine("uint _len = data.GetU32(offset);");
                            WriteLine("offset += 4;");
                            WriteLine($"{target} = System.Text.Encoding.UTF8.GetString(data.Slice(offset, (int)_len));");
                            WriteLine("offset += (int)_len;");
                            IndentLess();
                            WriteLine("}");
                            return;
                        }

                        // ---------- BYTES / VEC<U8> -------------------------------------
                        if (idlType == "bytes" || idlType == "vec<u8>")
                        {
                            WriteLine("{");
                            IndentAdd();
                            WriteLine("uint _len = data.GetU32(offset);");
                            WriteLine("offset += 4;");
                            WriteLine($"{target} = data.Slice(offset, (int)_len).ToArray();");
                            WriteLine("offset += (int)_len;");
                            IndentLess();
                            WriteLine("}");
                            return;
                        }

                        // ---------- VEC<T> ----------------------------------------------
                        if (idlType.StartsWith("vec<"))
                        {
                            string inner = idlType.Substring(4, idlType.Length - 5);
                            string listType = inner.ResolveCSharpType();
                            WriteLine($"uint _count = data.GetU32(offset);");
                            WriteLine("offset += 4;");
                            WriteLine($"{target} = new List<{listType}>();");
                            WriteLine("for(int i=0;i<_count;i++)");
                            BeginBlock();
                            string tmp = "_e";
                            WriteLine($"{listType} {tmp};");
                            EmitReadCode(inner, tmp);
                            WriteLine($"{target}.Add({tmp});");
                            EndBlock();
                            return;
                        }

                        // ---------- OPTION<T> -------------------------------------------
                        if (idlType.StartsWith("option<"))
                        {
                            string inner = idlType.Substring(7, idlType.Length - 8);
                            string innerCs = inner.ResolveCSharpType();
                            WriteLine("bool _present = data.GetBool(offset);");
                            WriteLine("offset += 1;");
                            WriteLine("if(_present)");
                            BeginBlock();
                            EmitReadCode(inner, target);
                            EndBlock();
                            WriteLine("else");
                            WriteLine($"    {target} = default({innerCs});");
                            return;
                        }

                        // ---------- STRUCT anidado --------------------------------------
                        // Asumimos que ya existe <Type>.Deserialize → devuelve bytes leídos
                        WriteLine($"{target} = new {idlType.ToPascalCase()}();");
                        WriteLine($"offset += {target}.Deserialize(data.Slice(offset));");
                        return;
                    }
            }
        }
    }
}

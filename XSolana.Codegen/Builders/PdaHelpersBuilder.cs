using System;
using System.Collections.Generic;
using System.Text;
using XSolana.Builders.Extensions;
using XSolana.Conventions;

namespace XSolana.Builders
{
    public sealed class PdaHelpersBuilder : CodeBuilderBase
    {
        public PdaHelpersBuilder(string className, string @namespace)
            : base(className, @namespace)
        {
            Includes =
            [
                "System",
                "System.Text",
                "System.Collections.Generic",
                "Solnet.Wallet",
                "Solnet.Programs.Utilities"
            ];
        }

        /*--------------------------------------------------------------*/
        public override string TransformText(ProgramDefinition program)
        {
            WriteFileHeader();
            BeginNamespace();

            BeginClass(program.Name.ToPascalCase() + "Pda", isStatic: true);

            /* -------- recopilar (Account, Seeds[]) SIN LINQ moderno ---*/
            var unique = new Dictionary<string, List<PdaSeedDefinition>>();

            foreach (var instr in program.Instructions)
            {
                foreach (var acc in instr.Accounts)
                {
                    if (acc.Pda == null) continue;

                    string key = acc.Name;                          // “UserStakeMeta”, …
                    if (!unique.ContainsKey(key))
                        unique[key] = acc.Pda.Seeds;
                }
            }

            /* -------- generar helpers --------------------------------*/
            foreach (var kvp in unique)
                EmitHelper(kvp.Key, kvp.Value);

            EndType();   // static class
            EndNamespace();
            return Code.ToString();
        }

        /*==============================================================*/
        private void EmitHelper(string accountName, IList<PdaSeedDefinition> seeds)
        {
            string method = "Derive" + accountName.ToPascalCase() + "Pda";

            /*---------- parámetros ------------------------------------*/
            var paramSet = new HashSet<string>(StringComparer.Ordinal);
            var paramList = new List<string>();

            for (int i = 0; i < seeds.Count; i++)
            {
                var s = seeds[i];
                if (s.Kind == PdaSeedKind.Account || s.Kind == PdaSeedKind.Arg)
                {
                    string root = s.Path.Split('.')[0].ToCamelCase();   // spl_token.mint → splToken
                    if (paramSet.Add(root))
                        paramList.Add("PublicKey " + root);
                }
            }
            paramList.Add("PublicKey programId");

            /*---------- firma -----------------------------------------*/
            WriteLine($"public static (PublicKey pda, byte bump) {method}({string.Join(", ", paramList)})");
            BeginBlock();

            /*---------- array de seeds --------------------------------*/
            WriteLine("List<byte[]> seeds = [];");
            BeginBlock();

            foreach (var s in seeds)
            {
                switch (s.Kind)
                {
                    /*---- CONST --------------------------------------*/
                    case PdaSeedKind.Const:
                        if (IsPrintableAscii(s.ConstBytes))
                        {
                            string str = Encoding.ASCII.GetString(s.ConstBytes);
                            WriteLine($"seeds.Add(Encoding.ASCII.GetBytes(\"{Escape(str)}\"));");
                        }
                        else
                        {
                            WriteLine($"seeds.Add(new byte[] {{ {string.Join(", ", s.ConstBytes)} }});");
                        }
                        break;

                    /*---- ACCOUNT / ARG ------------------------------*/
                    case PdaSeedKind.Account:
                    case PdaSeedKind.Arg:
                        string param = s.Path.Split('.')[0].ToCamelCase();
                        WriteLine($"seeds.Add({param}.KeyBytes);");
                        break;
                }
            }
            EndBlock(";");

            /*---------- derive & return ------------------------------*/
            WriteLine("if (!PublicKey.TryFindProgramAddress(seeds, programId, out var address, out var bump))");
            IndentAdd();
            WriteLine("throw new Exception(\"Failed to find PDA.\");");
            IndentLess();
            WriteReturn();
            WriteLine("return (address, bump);");

            EndBlock();         // método
            WriteReturn();
        }

        /*--------------------------------------------------------------*/
        private static bool IsPrintableAscii(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                byte b = bytes[i];
                if (b < 32 || b > 126) return false;        // fuera de rango printable
            }
            return true;
        }

        private static string Escape(string s) => s.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
}

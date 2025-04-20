using System.Linq;
using XSolana.Builders.Extensions;
using XSolana.Conventions;

namespace XSolana.Builders
{
    /// <summary>
    /// Generates C# code for Solana program instructions.
    /// </summary>
    public class InstructionBuilder : CodeBuilderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InstructionBuilder"/> class.
        /// </summary>
        /// <param name="className">A string representing the name of the class to be generated.</param>
        /// <param name="namespace">A string representing the namespace for the generated class.</param>
        public InstructionBuilder(string className, string @namespace)
            : base(className, @namespace) {
            Includes = [
                "Solnet.Rpc.Models",
                "Solnet.Wallet"
            ];
        }

        /// <summary>
        /// Transforms a ProgramDefinition into C# code for instruction builders.
        /// </summary>
        /// <param name="program">A <see cref="ProgramDefinition"/> object representing the program.</param>
        /// <returns>
        /// A string containing the generated C# code for the instruction builders.
        /// </returns>
        public override string TransformText(ProgramDefinition program)
        {
WriteFileHeader();
BeginNamespace();

            foreach (var instr in program.Instructions)
            {
    BeginClass(instr.Name.ToPascalCase() + "Builder", isStatic: true);

        WriteLine("public static TransactionInstruction Build(");
        IndentAdd();
                foreach (var account in instr.Accounts)
            WriteLine($"PublicKey {account.Name.ToCamelCase()},");
                foreach (var arg in instr.Args)
            WriteLine($"{arg.Type.ResolveCSharpType()} {arg.Name.ToCamelCase()},");

            WriteLine("PublicKey programId)");
        IndentLess();
        BeginBlock();
            WriteLine("var keys = new List<AccountMeta>");
            BeginBlock();
                foreach (var account in instr.Accounts)
                {
                    string metaType = account.IsSigner ? "Writable" : account.IsMut ? "Writable" : "ReadOnly";
                WriteLine($"AccountMeta.{metaType}({account.Name.ToCamelCase()}, {account.IsSigner.ToString().ToLower()}),");
                }
            EndBlock(";");
            WriteReturn();
                var argList = instr.Args.Any()
                    ? string.Join(", ", instr.Args.Select(a => a.Name.ToCamelCase()))
                    : string.Empty;
            WriteLine($"var data = {instr.Name.ToPascalCase()}Data.Encode({argList});");
            WriteReturn();
            WriteLine("return new TransactionInstruction");
            BeginBlock();
                WriteLine("ProgramId = programId,");
                WriteLine("Keys = keys,");
                WriteLine("Data = data");
            EndBlock(";");
        EndBlock();

    EndType();
            }

EndNamespace();

            return Code.ToString();
        }

    }
}

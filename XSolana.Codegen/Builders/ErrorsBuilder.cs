using System.Linq;
using XSolana.Builders.Extensions;
using XSolana.Conventions;

namespace XSolana.Builders
{
    public sealed class ErrorsBuilder : CodeBuilderBase
    {
        public ErrorsBuilder(string className, string @namespace)
            : base(className, @namespace)
        {
            Includes = new[] { "System", "System.Collections.Generic" };
        }

        public override string TransformText(ProgramDefinition program)
        {
            WriteFileHeader();
            BeginNamespace();

            //------------------------------------------------------------------
            // 1) Enum <Program>Error
            //------------------------------------------------------------------
            string enumName = program.Name.ToPascalCase() + "Error";
            BeginEnum(enumName);
            foreach (var err in program.Errors.OrderBy(e => e.Code))
                WriteLine($"{err.Name} = {err.Code},");
            EndType();
            WriteReturn();

            //------------------------------------------------------------------
            // 2) Extension helpers
            //------------------------------------------------------------------
            BeginClass(enumName + "Extensions", isStatic: true);

            // --- diccionario code → message -------------------------------
            WriteLine("private static readonly Dictionary<int,string> _messages = new()");
            BeginBlock();
            foreach (var err in program.Errors.OrderBy(e => e.Code))
                WriteLine($"{{ {err.Code}, \"{err.Message.Replace("\"", "\\\"")}\" }},");
            EndBlock(";");
            WriteReturn();

            // TryFromCode
            WriteLine($"public static bool TryFromCode(int code, out {enumName} value)");
            BeginBlock();
            WriteLine("if (Enum.IsDefined(typeof(" + enumName + "), code))");
            BeginBlock();
            WriteLine("value = (" + enumName + ")code;");
            WriteLine("return true;");
            EndBlock();
            WriteLine("value = default;");
            WriteLine("return false;");
            EndBlock();
            WriteReturn();

            // GetMessage
            WriteLine($"public static string GetMessage(this {enumName} e)");
            BeginBlock();
            WriteLine("return _messages.TryGetValue((int)e, out var m) ? m : string.Empty;");
            EndBlock();

            EndType(); // class
            EndNamespace();
            return Code.ToString();
        }

        // helper to write enum header
        private void BeginEnum(string name) => BeginType(name, "enum", isInternal: false);
    }
}

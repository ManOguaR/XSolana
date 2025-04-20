using System.Linq;
using XSolana.Builders.Extensions;
using XSolana.Conventions;

namespace XSolana.Builders
{
    /// <summary>
    /// Genera una clase *{Program}Service* con métodos async para:
    ///   • enviar transacciones de cada instrucción
    ///   • leer cuentas vía layouts
    ///   • suscribirse a eventos
    /// </summary>
    public sealed class ProgramServiceBuilder : CodeBuilderBase
    {
        public ProgramServiceBuilder(string className, string @namespace)
            : base(className, @namespace)
        {
            Includes =
            [
                "System",
                "System.Threading",
                "System.Threading.Tasks",
                "System.Collections.Generic",
                "Solnet.Rpc",
                "Solnet.Rpc.Core.Sockets",
                "Solnet.Rpc.Builders",
                "Solnet.Rpc.Models",
                "Solnet.Rpc.Types",
                "Solnet.Wallet",
                "Solnet.Programs.Utilities"
            ];
        }

        //------------------------------------------------------------------
        // ENTRY POINT
        //------------------------------------------------------------------
        public override string TransformText(ProgramDefinition program)
        {
            WriteFileHeader();
            BeginNamespace();

            string svcName = program.Name.ToPascalCase() + "Service";
            BeginClass(svcName, isStatic: false);

            //------------------------------------------------------------------
            // 1) Campos + ctor
            //------------------------------------------------------------------
            WriteLine("private readonly IRpcClient _rpc;");
            WriteLine("private readonly IStreamingRpcClient _stream;");
            WriteLine("private readonly Wallet _wallet;");
            WriteLine("public  PublicKey ProgramId { get; }");
            WriteReturn();

            WriteLine($"public {svcName}(IRpcClient rpc, IStreamingRpcClient stream, Wallet wallet, PublicKey programId)");
            BeginBlock();
            WriteLine("_rpc       = rpc  ?? throw new ArgumentNullException(nameof(rpc));");
            WriteLine("_stream    = stream ?? throw new ArgumentNullException(nameof(stream));");
            WriteLine("_wallet    = wallet ?? throw new ArgumentNullException(nameof(wallet));");
            WriteLine("ProgramId  = programId;");
            EndBlock();
            WriteReturn();

            //------------------------------------------------------------------
            // 2) Métodos por instrucción
            //------------------------------------------------------------------
            foreach (var instr in program.Instructions)
                EmitInstructionMethod(instr);

            //------------------------------------------------------------------
            // 3) Lectura de cuentas
            //------------------------------------------------------------------
            foreach (var acc in program.Accounts)
                EmitGetAccountMethod(acc);

            //------------------------------------------------------------------
            // 4) Subscripción a eventos
            //------------------------------------------------------------------
            EmitSubscribeEvents(program);

            EndType();   // service class
            EndNamespace();
            return Code.ToString();
        }

        /* ============================================================= */
        private void EmitInstructionMethod(InstructionDefinition instr)
        {
            string methodName = instr.Name.ToPascalCase() + "Async";

            // --- parámetros -------------------------------------------------
            var paramList = instr.Accounts
                                 .Select(a => $"PublicKey {a.Name.ToCamelCase()}")
                                 .Concat(instr.Args.Select(a => $"{a.Type.ResolveCSharpType()} {a.Name.ToCamelCase()}"))
                                 .Append("CancellationToken ct = default")
                                 .ToArray();

            WriteLine($"public async Task<string> {methodName}({string.Join(", ", paramList)})");
            BeginBlock();

            // --- Build instruction -----------------------------------------
            string builderClass = instr.Name.ToPascalCase() + "Builder";
            var argInvocation = string.Join(", ",
                                instr.Accounts.Select(a => a.Name.ToCamelCase())
                               .Concat(instr.Args.Select(a => a.Name.ToCamelCase()))
                               .Append("ProgramId"));

            WriteLine($"var ix = {builderClass}.Build({argInvocation});");

            // --- Compose + send transaction --------------------------------
            WriteLine("var blockHash = (await _rpc.GetLatestBlockHashAsync()).Result.Value.Blockhash;");

            WriteLine("var tx = new TransactionBuilder()");
            IndentAdd();
            WriteLine(".SetRecentBlockHash(blockHash)");
            WriteLine(".SetFeePayer(_wallet.Account.PublicKey)");
            WriteLine(".AddInstruction(ix)");
            WriteLine(".Build(_wallet.Account);");
            IndentLess();

            WriteLine("return (await _rpc.SendTransactionAsync(tx, true, Commitment.Confirmed)).Result;");
            EndBlock();
            WriteReturn();
        }

        /* ============================================================= */
        private void EmitGetAccountMethod(AccountDefinition acc)
        {
            string accName = acc.Name.ToPascalCase();
            WriteLine($"public async Task<{accName}> Get{accName}Async(PublicKey address, CancellationToken ct = default)");
            BeginBlock();
            WriteLine("var info = await _rpc.GetAccountInfoAsync(address, Commitment.Confirmed);");
            WriteLine("if (!info.WasSuccessful || info.Result?.Value?.Data == null) return null;");
            WriteLine("var dataBytes = Convert.FromBase64String(info.Result.Value.Data[0]);");
            WriteLine($"return {accName}.TryDeserialize(dataBytes, out var accObj) ? accObj : null;");
            EndBlock();
            WriteReturn();
        }

        /* ============================================================= */
        private void EmitSubscribeEvents(ProgramDefinition program)
        {
            string hub = program.Name.ToPascalCase() + "Events";

            WriteLine("public async Task<SubscriptionState> SubscribeEventsAsync(Action<object> handler)");
            BeginBlock();
            WriteLine("return await _stream.SubscribeProgramAsync(");
            IndentAdd();
            WriteLine("ProgramId.Key,");
            WriteLine("(state, result) =>");
            BeginBlock();
            WriteLine("if (result?.Value?.Account?.Data == null) return;");
            WriteLine("var dataBytes = Convert.FromBase64String(result.Value.Account.Data[0]);");
            WriteLine($"if ({hub}.TryParse(dataBytes, out var evt)) handler(evt!);");
            EndBlock(",");
            WriteLine("Commitment.Confirmed);");
            IndentLess();
            EndBlock();
        }
    }
}

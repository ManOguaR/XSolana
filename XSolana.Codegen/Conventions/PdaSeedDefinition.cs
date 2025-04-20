namespace XSolana.Conventions
{
    public enum PdaSeedKind { Const, Account, Arg }

    public sealed class PdaSeedDefinition
    {
        public PdaSeedKind Kind { get; set; }

        public byte[] ConstBytes { get; set; }

        public string Path { get; set; }

        public string Account { get; set; }
    }
}
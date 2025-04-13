using System.IO;
using XSolana.Builders;
using Xunit;

namespace XSolana.Codegen.Tests
{
    public class InstructionBuilderTests
    {
        [Fact]
        public void CanGenerateInstructionBuilder_FromBasicIdl()
        {
            // Arrange
            var json = File.ReadAllText("Samples/basic_idl.json");
            var program = new AnchorIdlParser().ParseFromJson(json);
            var builder = new InstructionBuilder($"{program.Name}InstructionBuilder", $"Generated.{program.Name}");

            // Act
            var code = builder.TransformText(program);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(code));
            Assert.Contains("public static TransactionInstruction Build(", code);
            Assert.Contains("AccountMeta.WritableSigner(payer, true)", code);
            Assert.Contains("var data = new byte[]", code);
        }

        [Fact]
        public void CanGenerateInstructionBuilder_FromFullIdl()
        {
            // Arrange
            var json = File.ReadAllText("Samples/full_idl.json");
            var program = new AnchorIdlParser().ParseFromJson(json);
            var builder = new InstructionBuilder($"{program.Name}InstructionBuilder", $"Generated.{program.Name}");

            // Act
            var code = builder.TransformText(program);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(code));
            Assert.Contains("public static class InitializeBuilder", code);
            Assert.Contains("AccountMeta.WritableSigner(authority, true)", code);
            Assert.Contains("TransactionInstruction", code);
            Assert.Contains("ProgramId = programId", code);
        }
    }
}

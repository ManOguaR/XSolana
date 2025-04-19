using System;
using System.IO;
using System.Linq;
using XSolana.Builders;
using XSolana.Conventions;
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
            //Assert.Contains("public static TransactionInstruction Build(", code);
            //Assert.Contains("AccountMeta.WritableSigner(payer, true)", code);
            Assert.Contains("AccountMeta.Writable(payer, true)", code);
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
            //Assert.Contains("public static class InitializeBuilder", code);
            //Assert.Contains("AccountMeta.WritableSigner(authority, true)", code);
            Assert.Contains("AccountMeta.Writable(authority, true)", code);
            Assert.Contains("TransactionInstruction", code);
            Assert.Contains("ProgramId = programId", code);
        }

        [Fact]
        public void CanGenerateInstructionBuilder_FromRealIdl()
        {
            // Arrange
            var json = File.ReadAllText("Samples/tcw_stakes.json");
            var parser = new AnchorIdlParser();
            ProgramDefinition model = parser.ParseFromJson(json); // ← ¡esto es clave!

            var outputDir = Path.Combine(Path.GetTempPath(), "XSolanaGenTests", "InstructionBuilders");
            if (Directory.Exists(outputDir))
                Directory.Delete(outputDir, true); // limpio el test antes

            // Act
            var builder = new InstructionBuilder($"{model.Name}InstructionBuilder", $"Generated.{model.Name}");
            var content = builder.TransformText(model);

            var outputPath = Path.Combine(outputDir, $"{Sanitize(model.Name)}.InstructionBuilder.g.cs");
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            File.WriteAllText(outputPath, content);

            // Assert
            Assert.True(File.Exists(outputPath));
            var generated = File.ReadAllText(outputPath);
            Assert.False(string.IsNullOrWhiteSpace(generated));
            //Assert.Contains($"class {model.Name}InstructionBuilder", generated);
            Assert.Contains("public", generated);
            Assert.Contains("InitializeBuilder", generated, StringComparison.OrdinalIgnoreCase); // instrucción real
        }

        private static string Sanitize(string name)
        {
            return Path.GetInvalidFileNameChars().Aggregate(name, (current, c) => current.Replace(c, '_'));
        }
    }
}

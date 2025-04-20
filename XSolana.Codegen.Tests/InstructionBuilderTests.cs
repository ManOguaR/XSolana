using System;
using System.IO;
using System.Linq;
using XSolana.Builders;
using Xunit;

namespace XSolana.Codegen.Tests
{
    public class InstructionDataBuilderTests
    {
        [Fact]
        public void CanGenerateInstructionData_FromBasicIdl()
        {
            // Arrange
            var json = File.ReadAllText("Samples/basic_idl.json");
            var program = new AnchorIdlParser().ParseFromJson(json);
            var builder = new InstructionDataBuilder($"{program.Name}InstructionData", $"Generated.{program.Name}");

            // Act
            var code = builder.TransformText(program);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(code));
            Assert.Contains("public static byte[] Encode(", code);
            Assert.Contains("var discriminator = new byte[]", code);
        }

        [Fact]
        public void CanGenerateInstructionData_FromRealIdl_TcwStakes()
        {
            // Arrange
            var json = File.ReadAllText("Samples/tcw_stakes.json");
            var program = new AnchorIdlParser().ParseFromJson(json);
            var builder = new InstructionDataBuilder($"{program.Name}InstructionData", $"Generated.{program.Name}");
            var output = builder.TransformText(program);

            // Assert basic structure
            Assert.Contains($"{program.Instructions.First().Name}Data", output, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("Encode(", output);
            // Debe incluir la llamada a WriteU64 para el argumento "amount"
            Assert.Contains("WriteU64", output);
            // Debe incluir el discriminador de 'stake_lock'
            Assert.Contains("75, 131, 36, 194, 172, 29, 122, 94", output);
        }
    }
}

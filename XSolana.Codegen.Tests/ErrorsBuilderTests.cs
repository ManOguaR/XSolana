using System.IO;
using XSolana.Builders;
using Xunit;

namespace XSolana.Codegen.Tests
{
    public class ErrorsBuilderTests
    {
        [Fact]
        public void CanGenerateErrorsEnum_AndExtensions_FromRealIdl()
        {
            // Arrange
            var json = File.ReadAllText("Samples/tcw_stakes.json");
            var program = new AnchorIdlParser().ParseFromJson(json);
            var builder = new ErrorsBuilder($"{program.Name}Error", $"Generated.{program.Name}");

            // Act
            var code = builder.TransformText(program);

            // Assert
            Assert.Contains($"enum {program.Name.ToPascalCase()}Error", code);
            // Debe tener al menos un miembro: StakingPeriodActive = 6004
            Assert.Contains("StakingPeriodActive = 6004", code);
            // Debe generar el diccionario de mensajes
            Assert.Contains("private static readonly Dictionary<int,string> _messages", code);
            Assert.Contains("\"Unstaking is not yet possible.\"", code);
        }
    }
}

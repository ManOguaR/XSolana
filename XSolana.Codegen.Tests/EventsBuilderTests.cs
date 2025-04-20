using System.IO;
using XSolana.Builders;
using Xunit;

namespace XSolana.Codegen.Tests
{
    public class EventsBuilderTests
    {
        [Fact]
        public void CanGenerateEventsHub_AndRecords_FromRealIdl()
        {
            // Arrange
            var json = File.ReadAllText("Samples/tcw_stakes.json");
            var program = new AnchorIdlParser().ParseFromJson(json);
            var builder = new EventsBuilder($"{program.Name}Events", $"Generated.{program.Name}");

            // Act
            var code = builder.TransformText(program);

            // Assert
            // Debe generar la clase estática hub
            Assert.Contains($"static class {program.Name.ToPascalCase()}Events", code);
            // Debe registrar AdminChanged
            Assert.Contains("AdminChangedEvent.TryDeserialize", code);
            // Debe generar el record AdminChangedEvent
            Assert.Contains("public class AdminChangedEvent", code);
            Assert.Contains("ulong disc", code);
        }
    }

}

using System.IO;
using Xunit;

namespace XSolana.Codegen.Tests
{
    public class AnchorIdlParserTests
    {
        [Fact]
        public void CanParseBasicIdl()
        {
            // Arrange
            var json = File.ReadAllText("Samples/basic_idl.json");
            var parser = new AnchorIdlParser();

            // Act
            var result = parser.ParseFromJson(json);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("basic_program", result.Name);
            Assert.NotEmpty(result.Instructions);
            Assert.All(result.Instructions, instr =>
            {
                Assert.False(string.IsNullOrWhiteSpace(instr.Name));
                Assert.NotNull(instr.Args);
                Assert.NotNull(instr.Accounts);
            });
        }

        [Fact]
        public void CanParseFullIdl()
        {
            // Arrange
            var json = File.ReadAllText("Samples/full_idl.json");
            var parser = new AnchorIdlParser();

            // Act
            var program = parser.ParseFromJson(json);

            // Assert: raíz
            Assert.NotNull(program);
            Assert.Equal("full_test_program", program.Name);
            Assert.Equal("0.1.0", program.Version);

            // Assert: instrucciones
            Assert.NotEmpty(program.Instructions);
            Assert.Contains(program.Instructions, i => i.Name == "initialize");

            // Assert: args y cuentas de la instrucción
            var instr = program.Instructions.Find(i => i.Name == "initialize");
            Assert.NotNull(instr);
            Assert.NotEmpty(instr.Args);
            Assert.NotEmpty(instr.Accounts);

            // Assert: accounts
            Assert.NotEmpty(program.Accounts);
            var acc = program.Accounts.Find(a => a.Name == "MyAccount");
            Assert.NotNull(acc);
            Assert.NotEmpty(acc.Type.Fields);

            // Assert: types (struct y enum)
            Assert.NotEmpty(program.Types);
            Assert.Contains(program.Types, t => t.Struct != null);
            Assert.Contains(program.Types, t => t.Enum != null);

            // Assert: events
            Assert.NotEmpty(program.Events);
            Assert.Contains(program.Events, e => e.Name == "MyEvent");

            // Assert: errores
            Assert.NotEmpty(program.Errors);
            Assert.Contains(program.Errors, err => err.Name == "Unauthorized");
        }
    }
}

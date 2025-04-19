using System.IO;
using System.Linq;
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
            Assert.Equal("0.1.0", result.Version);

            // Instrucciones
            Assert.NotEmpty(result.Instructions);
            Assert.All(result.Instructions, instr =>
            {
                Assert.False(string.IsNullOrWhiteSpace(instr.Name));
                Assert.NotNull(instr.Args);
                Assert.NotNull(instr.Accounts);

                // Validar flags de cuentas modernas
                foreach (var acc in instr.Accounts)
                {
                    _ = acc.IsMut;    // bool, default false si no está
                    _ = acc.IsSigner;
                }
            });

            // Estructura mínima: sin accounts ni types definidos
            Assert.Empty(result.Accounts);
            Assert.Empty(result.Types);
            Assert.Empty(result.Events);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void CanParseFullIdl()
        {
            // Arrange
            var json = File.ReadAllText("Samples/full_idl.json");
            var parser = new AnchorIdlParser();

            // Act
            var program = parser.ParseFromJson(json);

            // Assert: metadata raíz
            Assert.NotNull(program);
            Assert.Equal("full_test_program", program.Name);
            Assert.Equal("0.1.0", program.Version);

            // Instrucciones
            Assert.NotEmpty(program.Instructions);
            Assert.Contains(program.Instructions, i => i.Name == "initialize");

            var instr = program.Instructions.Find(i => i.Name == "initialize");
            Assert.NotNull(instr);
            Assert.NotEmpty(instr.Args);
            Assert.NotEmpty(instr.Accounts);

            // Validar flags modernos
            foreach (var metaAcc in instr.Accounts)
            {
                _ = metaAcc.IsMut;
                _ = metaAcc.IsSigner;
            }

            // Accounts (declaración + tipo separado)
            Assert.NotEmpty(program.Accounts);
            var declaredAcc = program.Accounts.Find(a => a.Name == "MyAccount");
            Assert.NotNull(declaredAcc);

            // Resolver tipo desde Types
            var type = program.Types.Find(t => t.Name == "MyAccount");
            Assert.NotNull(type);
            Assert.NotNull(type.Struct);
            Assert.NotEmpty(type.Struct.Fields);

            // Otros tipos
            Assert.Contains(program.Types, t => t.Struct != null && t.Name == "CustomStruct");
            Assert.Contains(program.Types, t => t.Enum != null && t.Name == "CustomEnum");

            // Eventos
            Assert.NotEmpty(program.Events);
            Assert.Contains(program.Events, e => e.Name == "MyEvent");

            // Errores
            Assert.NotEmpty(program.Errors);
            Assert.Contains(program.Errors, err => err.Name == "Unauthorized");
        }

        [Fact]
        public void CanParseRealIdl()
        {
            // Arrange
            var json = File.ReadAllText("Samples/tcw_stakes.json");
            var parser = new AnchorIdlParser();

            // Act
            var program = parser.ParseFromJson(json);

            // Raíz y metadatos
            Assert.NotNull(program);
            Assert.Equal("tcw_stakes", program.Name);
            Assert.Equal("0.1.0", program.Version);
            Assert.Equal("TCWLF1KfhWYzicBdUQbbSmwqHsW5DHrT6ga2CKoSc3n", program.Address);
            Assert.False(string.IsNullOrWhiteSpace(program.Description));

            // Instrucciones
            Assert.NotEmpty(program.Instructions);
            Assert.Equal(6, program.Instructions.Count);
            Assert.All(program.Instructions, instr =>
            {
                Assert.False(string.IsNullOrWhiteSpace(instr.Name));
                Assert.NotNull(instr.Discriminator);
                Assert.Equal(8, instr.Discriminator.Count);
                Assert.NotNull(instr.Accounts);
                Assert.All(instr.Accounts, acc =>
                {
                    Assert.False(string.IsNullOrWhiteSpace(acc.Name));
                    // Estos campos pueden estar o no
                    _ = acc.IsMut;
                    _ = acc.IsSigner;
                });
                Assert.NotNull(instr.Args);
            });

            // Instrucción específica: stake_lock
            var stakeLock = program.Instructions.FirstOrDefault(i => i.Name == "stake_lock");
            Assert.NotNull(stakeLock);
            Assert.Equal(2, stakeLock.Args.Count);
            Assert.Contains(stakeLock.Args, a => a.Name == "amount");
            Assert.Contains(stakeLock.Args, a => a.Name == "unlock_time");

            // Cuentas
            Assert.NotEmpty(program.Accounts);
            Assert.Equal(3, program.Accounts.Count);
            Assert.All(program.Accounts, acc =>
            {
                Assert.False(string.IsNullOrWhiteSpace(acc.Name));
                Assert.NotNull(acc.Discriminator);
                Assert.Equal(8, acc.Discriminator.Count);
            });

            // Tipos
            Assert.NotEmpty(program.Types);
            Assert.All(program.Types, t =>
            {
                Assert.False(string.IsNullOrWhiteSpace(t.Name));
                Assert.NotNull(t.Struct);
                Assert.NotEmpty(t.Struct.Fields);
            });

            // Types match accounts por nombre
            foreach (var account in program.Accounts)
            {
                var type = program.Types.FirstOrDefault(t => t.Name == account.Name);
                Assert.NotNull(type);
                Assert.NotNull(type.Struct);
            }

            // Validación de campos esperados en uno de los tipos
            var userStakeMeta = program.Types.First(t => t.Name == "UserStakeMeta");
            Assert.NotNull(userStakeMeta.Struct.Fields.FirstOrDefault(f => f.Name == "total_amount"));
            Assert.NotNull(userStakeMeta.Struct.Fields.FirstOrDefault(f => f.Type == "u64"));

            // Eventos
            Assert.NotEmpty(program.Events);
            Assert.Contains(program.Events, e => e.Name == "AdminChanged");

            // Errores
            Assert.NotEmpty(program.Errors);
            Assert.Contains(program.Errors, e => e.Code == 6004 && e.Name == "StakingPeriodActive");

            // Errores con mensaje
            Assert.All(program.Errors, e =>
            {
                Assert.False(string.IsNullOrWhiteSpace(e.Name));
                Assert.True(e.Code >= 6000);
                Assert.False(string.IsNullOrWhiteSpace(e.Message));
            });
        }
    }
}

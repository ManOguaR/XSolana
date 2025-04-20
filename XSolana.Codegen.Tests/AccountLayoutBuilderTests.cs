using System.IO;
using XSolana.Builders;
using Xunit;

namespace XSolana.Codegen.Tests
{
    public class AccountLayoutBuilderTests
    {
        [Fact]
        public void CanGenerateAccountLayouts_FromRealIdl_TcwStakes()
        {
            // Arrange
            var json = File.ReadAllText("Samples/tcw_stakes.json");
            var program = new AnchorIdlParser().ParseFromJson(json);
            var builder = new AccountLayoutBuilder($"{program.Name}Accounts", $"Generated.{program.Name}");

            // Act
            var code = builder.TransformText(program);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(code));
            // Debe generar la clase para UserStakeMeta con TryDeserialize
            Assert.Contains("class UserStakeMeta", code);
            Assert.Contains("TryDeserialize", code);
            // Debe incluir la comprobación de discriminador
            Assert.Contains("SequenceEqual(Discriminator)", code);
        }

        [Fact]
        public void GeneratedAccounts_FileCanBeWritten()
        {
            // Arrange
            var json = File.ReadAllText("Samples/tcw_stakes.json");
            var program = new AnchorIdlParser().ParseFromJson(json);
            var builder = new AccountLayoutBuilder($"{program.Name}Accounts", $"Generated.{program.Name}");
            var outputDir = Path.Combine(Path.GetTempPath(), "XSolanaGenTests", "Accounts");
            Directory.CreateDirectory(outputDir);

            // Act
            var content = builder.TransformText(program);
            var path = Path.Combine(outputDir, $"{program.Name}.Accounts.g.cs");
            File.WriteAllText(path, content);

            // Assert
            Assert.True(File.Exists(path));
            var generated = File.ReadAllText(path);
            Assert.Contains("public static readonly byte[] Discriminator", generated);
        }
    }
}

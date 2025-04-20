using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using XSolana.Conventions;

namespace XSolana
{
    /// <summary>
    /// AnchorIdlCodeGen is a MSBuild task that generates C# code from Anchor IDL files.
    /// </summary>
    public class AnchorIdlCodeGen : Task
    {
        /// <summary>
        /// The IDL files to process.
        /// </summary>
        [Required]
        public ITaskItem[] IdlFiles { get; set; } = Array.Empty<ITaskItem>();

        /// <summary>
        /// The output directory where the generated C# files will be saved.
        /// </summary>
        [Required]
        public string OutputDir { get; set; } = string.Empty;

        /// <summary>
        /// The project directory where the IDL files are located.
        /// </summary>
        public string ProjectDir { get; set; }

        /// <summary>
        /// Executes the task.
        /// </summary>
        /// <returns>A boolean value indicating whether the task was successful.</returns>
        public override bool Execute()
        {
            try
            {
                Log.LogMessage(MessageImportance.High, $"[XSolana] Procesando {IdlFiles.Length} IDL(s)...");

                var parser = new AnchorIdlParser();

                foreach (var item in IdlFiles)
                {
                    var idlPath = item.ItemSpec;
                    var fileName = Path.GetFileNameWithoutExtension(idlPath);

                    Log.LogMessage(MessageImportance.Low, $"[XSolana] → {idlPath}");

                    var program = parser.ParseFromFile(idlPath);

                    //RunConstantsBuilder(program, OutputDir);
                    RunInstructionDataBuilder(program, OutputDir);
                    RunInstructionBuilder(program, OutputDir);
                    RunAccountLayouts(program, OutputDir);
                    RunErrorsBuilder(program, OutputDir);
                    RunEventsBuilder(program, OutputDir);
                    RunPdaHelpersBuilder(program, OutputDir);
                    RunProgramServiceBuilder(program, OutputDir);
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex, true);
                return false;
            }
        }

        private void RunInstructionDataBuilder(ProgramDefinition model, string outputDir)
        {
            var builder = new Builders.InstructionDataBuilder($"{model.Name}InstructionData", $"Generated.{model.Name}");
            var content = builder.TransformText(model);

            var outputPath = Path.Combine(outputDir, $"{model.Name}.InstructionDataBuilder.g.cs");
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            File.WriteAllText(outputPath, content);

            Log.LogMessage(MessageImportance.Low, $"[XSolana] InstructionDataBuilder generado en: {outputPath}");
        }

        private void RunInstructionBuilder(ProgramDefinition model, string outputDir)
        {
            var builder = new Builders.InstructionBuilder($"{model.Name}InstructionBuilder", $"Generated.{model.Name}");
            var content = builder.TransformText(model);

            var outputPath = Path.Combine(outputDir, $"{Sanitize(model.Name)}.InstructionBuilder.g.cs");
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            File.WriteAllText(outputPath, content);

            Log.LogMessage(MessageImportance.Low, $"[XSolana] InstructionBuilder generado en: {outputPath}");
        }

        private void RunErrorsBuilder(ProgramDefinition model, string outputDir)
        {
            var builder = new Builders.ErrorsBuilder($"{model.Name}Errors", $"Generated.{model.Name}");
            var content = builder.TransformText(model);
            var path = Path.Combine(outputDir, $"{model.Name}.Errors.g.cs");
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, content);
            Log.LogMessage(MessageImportance.Low, $"[XSolana] ErrorsBuilder generado en: {path}");
        }

        private void RunEventsBuilder(ProgramDefinition model, string outputDir)
        {
            var builder = new Builders.EventsBuilder($"{model.Name}Events", $"Generated.{model.Name}");
            var content = builder.TransformText(model);
            var path = Path.Combine(outputDir, $"{model.Name}.Events.g.cs");
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, content);
            Log.LogMessage(MessageImportance.Low, $"[XSolana] EventsBuilder generado en: {path}");
        }

        private void RunPdaHelpersBuilder(ProgramDefinition model, string outputDir)
        {
            var builder = new Builders.PdaHelpersBuilder($"{model.Name}Pda", $"Generated.{model.Name}");
            var content = builder.TransformText(model);
            var path = Path.Combine(outputDir, $"{model.Name}.PdaHelpers.g.cs");
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, content);
            Log.LogMessage(MessageImportance.Low, $"[XSolana] PdaHelpersBuilder generado en: {path}");
        }

        private void RunProgramServiceBuilder(ProgramDefinition model, string outputDir)
        {
            var builder = new Builders.ProgramServiceBuilder($"{model.Name}Service", $"Generated.{model.Name}");
            var content = builder.TransformText(model);
            var path = Path.Combine(outputDir, $"{model.Name}.Service.g.cs");
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, content);
            Log.LogMessage(MessageImportance.Low, $"[XSolana] ProgramServiceBuilder generado en: {path}");
        }
        private void RunAccountLayouts(ProgramDefinition model, string outputDir)
        {
            // Nombre de la clase estática contenedora y del namespace “Generated.<program>”
            var builder = new Builders.AccountLayoutBuilder(
                $"{model.Name}Accounts", $"Generated.{model.Name}");

            var content = builder.TransformText(model);

            var outputPath = Path.Combine(outputDir, $"{Sanitize(model.Name)}.Accounts.g.cs");
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            File.WriteAllText(outputPath, content);

            Log.LogMessage(MessageImportance.Low,
                $"[XSolana] AccountLayouts generado en: {outputPath}");
        }

        private static string Sanitize(string name)
        {
            return Path.GetInvalidFileNameChars().Aggregate(name, (current, c) => current.Replace(c, '_'));
        }
    }
}

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

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
                    //RunInstructionBuilder(program, OutputDir);
                    // RunProgramService(program, OutputDir);
                    // RunAccountLayouts(program, OutputDir);
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex, true);
                return false;
            }
        }

        //private void RunConstantsBuilder(ProgramDefinition model, string outputDir)
        //{
        //    var generator = new ConstantsBuilder(); // clase generada desde Constants.tt (preprocesada)
        //    generator.Model = model;

        //    var content = generator.TransformText();

        //    var outputPath = Path.Combine(outputDir, $"{Sanitize(model.Name)}.Constants.g.cs");
        //    Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

        //    File.WriteAllText(outputPath, content);

        //    Log.LogMessage(MessageImportance.Low, $"[XSolana] Constants generado en: {outputPath}");
        //}

        //private void RunInstructionBuilder(ProgramDefinition model, string outputDir)
        //{
        //    var generator = new InstructionBuilder(); // generado desde InstructionBuilder.tt
        //    generator.Model = model;

        //    var content = generator.TransformText();

        //    var outputPath = Path.Combine(outputDir, $"{Sanitize(model.Name)}.InstructionBuilder.g.cs");
        //    Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

        //    File.WriteAllText(outputPath, content);

        //    Log.LogMessage(MessageImportance.Low, $"[XSolana] InstructionBuilder generado en: {outputPath}");
        //}

        // private void RunProgramService(...) { ... }
        // private void RunAccountLayouts(...) { ... }

        private static string Sanitize(string name)
        {
            return Path.GetInvalidFileNameChars().Aggregate(name, (current, c) => current.Replace(c, '_'));
        }
    }
}

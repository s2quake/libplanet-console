using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace LibplanetConsole.Console.Tests;

internal static class ConsoleApplicationCompiler
{
    public static void CompileCode(string code, string assemblyName, string assemblyPath)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var references = new MetadataReference[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.Console).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Thread).Assembly.Location),
            MetadataReference.CreateFromFile(GetRuntimeLibraryPath("System.Runtime.dll")),
        };

        var options = new CSharpCompilationOptions(OutputKind.ConsoleApplication);
        var compilation = CSharpCompilation.Create(
            assemblyName, [syntaxTree], references, options);

        using var fs = new FileStream(assemblyPath, FileMode.Create, FileAccess.Write);
        var result = compilation.Emit(fs);

        if (!result.Success)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Compilation failed.");
            foreach (var diagnostic in result.Diagnostics)
            {
                sb.AppendLine(diagnostic.ToString());
            }

            throw new InvalidOperationException(sb.ToString());
        }

        fs.Close();
        CreateRuntimeConfig(assemblyPath);
    }

    private static void CreateRuntimeConfig(string assemblyPath)
    {
        var name = Path.GetFileNameWithoutExtension(assemblyPath);
        var directory = Path.GetDirectoryName(assemblyPath)
            ?? throw new InvalidOperationException("Invalid assembly path.");
        var configPath = Path.Combine(directory, $"{name}.runtimeconfig.json");
        var runtimeConfig = new
        {
            runtimeOptions = new
            {
                tfm = "net8.0",
                framework = new
                {
                    name = "Microsoft.NETCore.App",
                    version = "8.0.0",
                },
            },
        };

        var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
        var runtimeConfigJson = JsonSerializer.Serialize(runtimeConfig, jsonOptions);
        File.WriteAllText(configPath, runtimeConfigJson);
    }

    private static string GetRuntimeLibraryPath(string name)
        => Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), name);
}

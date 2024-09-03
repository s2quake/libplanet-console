using JSSoft.Commands;

namespace LibplanetConsole.Nodes.Executable.EntryCommands;

internal sealed class SchemaCommand : CommandBase
{
    [CommandProperty("output")]
    public string OutputPath { get; set; } = string.Empty;

    protected override void OnExecute()
    {
        var schemaBuilder = new ApplicationSettingsSchemaBuilder();
        var json = schemaBuilder.Build();
        var outputPath = OutputPath;
        if (outputPath == string.Empty)
        {
            Out.WriteLine(json);
        }
        else
        {
            var directory = Path.GetDirectoryName(outputPath)
                ?? throw new InvalidOperationException("The directory is not found.");
            if (Directory.Exists(directory) is false)
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(outputPath, json);
        }
    }
}

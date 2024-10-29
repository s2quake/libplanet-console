using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Options;

namespace LibplanetConsole.Client.Executable.EntryCommands;

[CommandSummary("Creates a schema of the application settings.")]
[Category("Tools")]
internal sealed class SchemaCommand : CommandBase
{
    protected override void OnExecute()
    {
        var schemaBuilder = OptionsSchemaBuilder.Create();
        var json = schemaBuilder.Build();
        var colorizedString = JsonUtility.ToColorizedString(json);
        Out.WriteLine(colorizedString);
    }
}

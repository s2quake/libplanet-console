using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.Nodes.Executable.EntryCommands;

[CommandSummary("Creates a schema of the application settings.")]
[Category("Tools")]
internal sealed class SchemaCommand : CommandBase
{
    protected override void OnExecute()
    {
        var schemaBuilder = new ApplicationSettingsSchemaBuilder();
        var json = schemaBuilder.Build();
        var colorizedString = JsonUtility.ToColorizedString(json);
        Out.WriteLine(colorizedString);
    }
}

using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.Nodes.Executable.EntryCommands;

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

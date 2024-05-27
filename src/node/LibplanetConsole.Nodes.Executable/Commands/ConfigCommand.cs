using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.Nodes.Executable.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Get and set options")]
[method: ImportingConstructor]
internal sealed class ConfigCommand(IApplicationConfigurations configurations) : CommandBase
{
    [CommandPropertyRequired(DefaultValue = "")]
    public string Key { get; set; } = string.Empty;

    [CommandPropertyRequired(DefaultValue = "")]
    public string Value { get; set; } = string.Empty;

    protected override void OnExecute()
    {
        if (Key == string.Empty && Value == string.Empty)
        {
            foreach (var key in configurations)
            {
                Out.Write($"{key}={configurations[key]}");
            }
        }
        else if (Value == string.Empty)
        {
            Out.WriteLine($"{configurations[Key]}");
        }
        else
        {
            configurations[Key] = Value;
        }
    }
}

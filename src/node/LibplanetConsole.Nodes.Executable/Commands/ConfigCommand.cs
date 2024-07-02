using System.ComponentModel.Composition;
using System.Text;
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
            var sb = new StringBuilder();
            foreach (var key in configurations)
            {
                sb.AppendLine($"{key}={configurations[key]}");
            }

            Out.Write(sb.ToString());
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

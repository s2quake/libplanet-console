using System.ComponentModel.Composition;
using JSSoft.Commands;

namespace OnBoarding.ConsoleHost.Commands;

[Export(typeof(ICommand))]
[method: ImportingConstructor]
sealed class ConfigCommand(ApplicationConfigurations configurations) : CommandBase
{
    [CommandPropertyRequired(DefaultValue = "")]
    public string Key { get; set; } = string.Empty;

    [CommandPropertyRequired(DefaultValue = "")]
    public string Value { get; set; } = string.Empty;

    protected override void OnExecute()
    {
        if (Key == string.Empty && Value == string.Empty)
        {
            foreach (var item in configurations.Descriptors)
            {
                Out.Write(item);
            }
        }
        else if (Value == string.Empty)
        {
            Out.WriteLine($"{configurations.GetValue(Key)}");
        }
        else
        {
            configurations.SetValue(Key, Value);
        }
    }
}

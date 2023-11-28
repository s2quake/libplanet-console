
using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using JSSoft.Library.Commands.Extensions;
using JSSoft.Library.Terminals;

namespace OnBoarding.ConsoleHost;

[Export]
sealed class SystemTerminal : SystemTerminalBase
{
    private readonly Application _application;
    private readonly CommandContext _commandContext;

    [ImportingConstructor]
    public SystemTerminal(Application application, CommandContext commandContext)
    {
        _application = application;
        _commandContext = commandContext;
        _commandContext.Owner = application;
        Prompt = "libplanet $ ";
    }

    protected override string FormatPrompt(string prompt)
    {
        return prompt;
    }

    protected override string[] GetCompletion(string[] items, string find)
    {
        return _commandContext.GetCompletion(items, find);
    }

    protected override Task OnExecuteAsync(string command, CancellationToken cancellationToken)
    {
        return _commandContext.ExecuteAsync(command, cancellationToken);
    }

    protected override void OnInitialize(TextWriter @out, TextWriter error)
    {
        _commandContext.Out = @out;
        _commandContext.Error = error;
    }
}

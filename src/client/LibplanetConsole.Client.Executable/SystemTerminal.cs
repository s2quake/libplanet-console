using JSSoft.Commands.Extensions;
using JSSoft.Terminals;
using LibplanetConsole.Common;

namespace LibplanetConsole.Client.Executable;

[Export]
internal sealed class SystemTerminal : SystemTerminalBase
{
    private readonly CommandContext _commandContext;

    public SystemTerminal(IApplication application, CommandContext commandContext)
    {
        _commandContext = commandContext;
        _commandContext.Owner = application;
        Prompt = "libplanet-client $ ";
    }

    protected override string FormatPrompt(string prompt) => prompt;

    protected override string[] GetCompletion(string[] items, string find)
        => _commandContext.GetCompletion(items, find);

    protected override Task OnExecuteAsync(string command, CancellationToken cancellationToken)
        => _commandContext.ExecuteAsync(command, cancellationToken);

    protected override void OnInitialize(TextWriter @out, TextWriter error)
    {
        _commandContext.Out = @out;
        _commandContext.Error = error;
    }
}

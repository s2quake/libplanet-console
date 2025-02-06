using JSSoft.Commands;

namespace LibplanetConsole.Alias.Commands;

[CommandSummary("Prints the address of the node")]
internal sealed class AliasRemoveCommand(AliasCommand aliasCommand, IAliasCollection aliases)
    : CommandAsyncBase(aliasCommand, "remove")
{
    [CommandPropertyRequired]
    [CommandSummary("If set, prints the address in raw format.")]
    public string Alias { get; set; } = string.Empty;

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        await aliases.RemoveAsync(Alias, cancellationToken);
    }
}

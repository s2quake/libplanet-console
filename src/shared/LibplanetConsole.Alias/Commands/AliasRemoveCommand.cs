using JSSoft.Commands;

namespace LibplanetConsole.Alias.Commands;

[CommandSummary("Removes an alias from the collection")]
internal sealed class AliasRemoveCommand(AliasCommand aliasCommand, IAliasCollection aliases)
    : CommandAsyncBase(aliasCommand, "remove")
{
    [CommandPropertyRequired]
    [CommandSummary("Specifies the alias to be removed.")]
    public string Alias { get; set; } = string.Empty;

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        await aliases.RemoveAsync(Alias, cancellationToken);
    }
}

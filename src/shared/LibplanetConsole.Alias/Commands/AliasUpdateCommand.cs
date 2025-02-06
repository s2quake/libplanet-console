using JSSoft.Commands;

namespace LibplanetConsole.Alias.Commands;

[CommandSummary("Prints the address of the node")]
internal sealed class AliasUpdateCommand(AliasCommand aliasCommand, IAliasCollection aliases)
    : CommandAsyncBase(aliasCommand, "update")
{
    [CommandPropertyRequired]
    [CommandSummary("If set, prints the address in raw format.")]
    public string Alias { get; set; } = string.Empty;

    [CommandProperty]
    public Address? Address { get; set; }

    [CommandProperty]
    public string[]? Tags { get; set; } = [];

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        if (Address is null && Tags is null)
        {
            throw new InvalidOperationException("Either address or tags should be specified.");
        }

        var oldAliasInfo = aliases[Alias];
        var newAliasInfo = oldAliasInfo;

        if (Address is { } address)
        {
            newAliasInfo = newAliasInfo with { Address = address };
        }

        if (Tags is { } tags)
        {
            newAliasInfo = newAliasInfo with { Tags = tags };
        }

        if (oldAliasInfo.Equals(newAliasInfo))
        {
            throw new InvalidOperationException("There is no change.");
        }

        await aliases.UpdateAsync(newAliasInfo, cancellationToken);
    }
}

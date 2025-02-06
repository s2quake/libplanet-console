using JSSoft.Commands;

namespace LibplanetConsole.Alias.Commands;

[CommandSummary("Updates the alias with a new address or tags.")]
internal sealed class AliasUpdateCommand(AliasCommand aliasCommand, IAliasCollection aliases)
    : CommandAsyncBase(aliasCommand, "update")
{
    [CommandPropertyRequired]
    [CommandSummary("Specifies the alias to be updated.")]
    public string Alias { get; set; } = string.Empty;

    [CommandProperty]
    [CommandSummary("Specifies the new address to associate with the alias.")]
    public Address? Address { get; set; }

    [CommandProperty]
    [CommandSummary("Specifies the new tags to associate with the alias.")]
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

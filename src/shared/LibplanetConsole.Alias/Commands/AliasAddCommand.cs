using JSSoft.Commands;

namespace LibplanetConsole.Alias.Commands;

[CommandSummary("Adds an alias")]
internal sealed class AliasAddCommand(AliasCommand aliasCommand, IAliasCollection aliases)
    : CommandAsyncBase(aliasCommand, "add")
{
    [CommandPropertyRequired]
    [CommandSummary("Specifies the alias to add.")]
    public string Alias { get; set; } = string.Empty;

    [CommandPropertyRequired]
    [CommandSummary("Specifies the address to add.")]
    public Address Address { get; set; }

    [CommandProperty]
    [CommandSummary("Specifies the tags to add.")]
    public string[] Tags { get; set; } = [];

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        var aliasInfo = new AliasInfo
        {
            Alias = Alias,
            Address = Address,
            Tags = Tags,
        };
        await aliases.AddAsync(aliasInfo, cancellationToken);
    }
}

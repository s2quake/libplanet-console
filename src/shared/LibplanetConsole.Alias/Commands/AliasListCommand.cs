using JSSoft.Commands;
using JSSoft.Commands.Extensions;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Alias.Commands;

[CommandSummary("Prints the address of the node")]
internal sealed class AliasListCommand(AliasCommand aliasCommand, IAliasCollection aliases)
    : CommandBase(aliasCommand, "ls")
{
    [CommandPropertySwitch("raw")]
    [CommandSummary("If set, prints the address in raw format.")]
    public bool IsRaw { get; set; }

    protected override void OnExecute()
    {
        var aliasInfos = aliases.GetAliasInfos();
        if (IsRaw is true)
        {
            var tableDataBuilder = new TableDataBuilder(2);
            foreach (var aliasInfo in aliasInfos)
            {
                tableDataBuilder.Add([$"{aliasInfo.Alias}", aliasInfo.Address]);
            }

            Out.Print(tableDataBuilder);
        }
        else
        {
            Out.WriteLineAsJson(aliasInfos);
        }
    }
}

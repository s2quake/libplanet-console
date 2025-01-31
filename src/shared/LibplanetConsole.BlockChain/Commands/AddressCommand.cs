using JSSoft.Commands;
using JSSoft.Commands.Extensions;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.BlockChain.Commands;

[CommandSummary("Prints the address of the node")]
internal sealed class AddressCommand(IAddressCollection addresses) : CommandBase
{
    [CommandPropertySwitch("raw")]
    [CommandSummary("If set, prints the address in raw format.")]
    public bool IsRaw { get; set; }

    protected override void OnExecute()
    {
        var addressInfos = addresses.GetAddressInfos();
        if (IsRaw is true)
        {
            var tableDataBuilder = new TableDataBuilder(2);
            foreach (var addressInfo in addressInfos)
            {
                tableDataBuilder.Add([$"{addressInfo.Alias}", addressInfo.Address]);
            }

            Out.Print(tableDataBuilder);
        }
        else
        {
            Out.WriteLineAsJson(addressInfos);
        }
    }
}

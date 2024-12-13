using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Console.Commands;

[CommandSummary("Prints the address list")]
internal sealed class AddressCommand(IAddressCollection addresses) : CommandBase
{
    protected override void OnExecute()
    {
        var addressInfos = addresses.GetAddressInfos();
        Out.WriteLineAsJson(addressInfos);
    }
}

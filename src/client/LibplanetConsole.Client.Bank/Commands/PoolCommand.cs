// using System.ComponentModel;
// using JSSoft.Commands;
// using LibplanetConsole.Common.Extensions;

// namespace LibplanetConsole.Client.Bank.Commands;

// [CommandSummary("Retrieve pool information.")]
// [Category("Bank")]
// internal sealed partial class PoolCommand(
//     IClient client, IBank bank) : CommandAsyncBase
// {
//     public override bool IsEnabled => client.IsRunning is true;

//     protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
//     {
//         var poolInfo = await bank.GetPoolAsync(cancellationToken);
//         await Out.WriteLineAsJsonAsync(poolInfo);
//     }
// }

// using System.ComponentModel;
// using JSSoft.Commands;
// using LibplanetConsole.Bank.Services;
// using LibplanetConsole.Common.Extensions;

// namespace LibplanetConsole.Console.Bank.Commands;

// [CommandSummary("Transfer NCG to specific address.")]
// [Category("Bank")]
// internal sealed partial class TransferCommand(IApplication application)
//     : CommandAsyncBase()
// {
//     [CommandPropertyRequired]
//     public string TargetAddress { get; set; } = string.Empty;

//     [CommandPropertyRequired]
//     public decimal Amount { get; set; }

//     [CommandProperty]
//     public string Address { get; set; } = string.Empty;

//     protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
//     {
//         var addressable = application.GetAddressable(Address);
//         if (addressable is IServiceProvider serviceProvider &&
//             serviceProvider.GetService(typeof(IBank)) is IBank bank)
//         {
//             var targetAddressable = application.GetAddressable(TargetAddress);
//             var options = new TransferOptions
//             {
//                 TargetAddress = targetAddressable.Address,
//                 Amount = Amount,
//             };
//             var balanceInfo = await bank.TransferAsync(options, cancellationToken);
//             await Out.WriteLineAsJsonAsync(balanceInfo);
//         }
//         else
//         {
//             throw new InvalidOperationException("Bank service is not available.");
//         }
//     }
// }

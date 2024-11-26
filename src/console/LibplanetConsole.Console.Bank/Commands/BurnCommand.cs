// using System.ComponentModel;
// using JSSoft.Commands;
// using LibplanetConsole.Bank.Services;
// using LibplanetConsole.Common.Extensions;

// namespace LibplanetConsole.Console.Bank.Commands;

// [CommandSummary("Burn NCG from specific address.")]
// [Category("Bank")]
// internal sealed partial class BurnCommand(IApplication application)
//     : CommandAsyncBase()
// {
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
//             var options = new BurnOptions
//             {
//                 Amount = Amount,
//             };
//             var balanceInfo = await bank.BurnAsync(options, cancellationToken);
//             await Out.WriteLineAsJsonAsync(balanceInfo);
//         }
//         else
//         {
//             throw new InvalidOperationException("Bank service is not available.");
//         }
//     }
// }

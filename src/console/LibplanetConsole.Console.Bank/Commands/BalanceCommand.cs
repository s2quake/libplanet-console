// using System.ComponentModel;
// using JSSoft.Commands;
// using LibplanetConsole.Common.Extensions;

// namespace LibplanetConsole.Console.Bank.Commands;

// [CommandSummary("Display balance of specific address.")]
// [Category("Bank")]
// internal sealed partial class BalanceCommand(IServiceProvider serviceProvider)
//     : CommandAsyncBase()
// {
//     [CommandProperty]
//     public Address Address { get; set; }

//     protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
//     {
//         var addressable = serviceProvider.GetAddressable(Address);
//         if (addressable is IServiceProvider serviceProvider &&
//             serviceProvider.GetService(typeof(IBank)) is IBank bank)
//         {
//             var address = Address;
//             var balanceInfo = await bank.GetBalanceAsync(address, cancellationToken);
//             await Out.WriteLineAsJsonAsync(balanceInfo);
//         }
//         else
//         {
//             throw new InvalidOperationException("Bank service is not available.");
//         }
//     }

//     private IAddressable GetAddressable(Address address)
//     {
//         if (nodes.Contains(address) is true)
//         {
//             return nodes[address];
//         }

//         if (clients.Contains(address) is true)
//         {
//             return clients[address];
//         }

//         throw new ArgumentException("Invalid address.");
//     }
// }

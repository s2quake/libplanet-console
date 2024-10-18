// using System.ComponentModel;
// using JSSoft.Commands;
// using LibplanetConsole.Common.Extensions;
// using LibplanetConsole.Console.Extensions;
// using LibplanetConsole.Delegation.Services;
// using Microsoft.Extensions.DependencyInjection;

// namespace LibplanetConsole.Console.Delegation.Commands;

// [CommandSummary("Delegate NCG to validator for delegation.")]
// [Category("Delegation")]
// internal sealed partial class DelegateCommand
//     : CommandAsyncBase
// {
//     [CommandPropertyRequired(DefaultValue = "")]
//     public Address Address { get; set; }

//     [CommandPropertyRequired(DefaultValue = "")]
//     public Address NodeAddress { get; set; }

//     [CommandPropertyRequired(DefaultValue = 10)]
//     public double Amount { get; set; }

//     protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
//     {
//         var addressable = application.GetAddressable(Address);
//         var node = addressable.GetNode();
//         if (addressable is IServiceProvider serviceProvider &&
//             serviceProvider.GetRequiredService<IDelegation>() is { } delegation)
//         {
//             var options = new DelegateOptions
//             {
//                 NodeAddress = node.Address,
//                 Amount = Amount,
//             };
//             var validatorInfo = await delegation.DelegateAsync(options, cancellationToken);
//             await Out.WriteLineAsJsonAsync(validatorInfo);
//         }
//         else
//         {
//             throw new InvalidOperationException("The application does not support delegation.");
//         }
//     }
// }

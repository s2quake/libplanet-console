// using System.ComponentModel;
// using JSSoft.Commands;
// using LibplanetConsole.Common.Extensions;
// using LibplanetConsole.Console.Extensions;
// using LibplanetConsole.Delegation.Services;
// using Microsoft.Extensions.DependencyInjection;

// namespace LibplanetConsole.Console.Delegation.Commands;

// [CommandSummary("Undelegate Share from validator.")]
// [Category("Delegation")]
// internal sealed class UndelegateCommand(IApplication application)
//     : CommandAsyncBase()
// {
//     [CommandPropertyRequired(DefaultValue = "")]
//     public string Address { get; set; } = string.Empty;

//     [CommandPropertyRequired(DefaultValue = "")]
//     public string NodeAddress { get; set; } = string.Empty;

//     [CommandPropertyRequired(DefaultValue = 500)]
//     public long ShareAmount { get; set; }

//     protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
//     {
//         var addressable = application.GetAddressable(Address);
//         var node = addressable.GetNode();
//         if (addressable is IServiceProvider serviceProvider &&
//             serviceProvider.GetRequiredService<IDelegation>() is { } delegation)
//         {
//             var options = new UndelegateOptions
//             {
//                 NodeAddress = node.Address,
//                 ShareAmount = ShareAmount,
//             };
//             var validatorInfo = await delegation.UndelegateAsync(options, cancellationToken);
//             await Out.WriteLineAsJsonAsync(validatorInfo);
//         }
//         else
//         {
//             throw new InvalidOperationException("Delegation service is not available.");
//         }
//     }
// }

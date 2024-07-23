// using System.ComponentModel.Composition;
// using Bencodex;
// using LibplanetConsole.Common.Extensions;
// using LibplanetConsole.Frameworks;
// using LibplanetConsole.Nodes;
// using Nekoyume.Action.Guild;

// namespace LibplanetConsole.Clients.Executable;

// [Export(typeof(IApplicationService))]
// internal sealed class GuildEventTracer : IApplicationService
// {
//     private static readonly Codec _codec = new();

//     private readonly Dictionary<string, Func<TransactionInfo, int, Task>> _actionHandlers;
//     private readonly IClient _client;
//     private readonly IBlockChain _blockChain;

//     [ImportingConstructor]
//     public GuildEventTracer(IClient client, IBlockChain blockChain)
//     {
//         _client = client;
//         _blockChain = blockChain;
//         _actionHandlers = new()
//         {
//             { MakeGuild.TypeIdentifier, OnGuildCreatedAsync },
//             { RemoveGuild.TypeIdentifier, OnGuildDeletedAsync },
//             { ApplyGuild.TypeIdentifier, OnGuildJoinRequestedAsync },
//             { CancelGuildApplication.TypeIdentifier, OnGuildJoinCancelledAsync },
//             { AcceptGuildApplication.TypeIdentifier, OnGuildJoinAcceptedAsync },
//             { RejectGuildApplication.TypeIdentifier, OnGuildJoinRejectedAsync },
//             { BanGuildMember.TypeIdentifier, OnGuildMemberBannedAsync },
//             { UnbanGuildMember.TypeIdentifier, OnGuildMemberUnbannedAsync },
//         };
//     }

//     public Task InitializeAsync(
//         IServiceProvider serviceProvider, CancellationToken cancellationToken)
//     {
//         _client.BlockAppended += Client_BlockAppended;
//         return Task.CompletedTask;
//     }

//     public ValueTask DisposeAsync()
//     {
//         _client.BlockAppended -= Client_BlockAppended;
//         return ValueTask.CompletedTask;
//     }

//     private static bool ContainsAction(
//         BlockInfo blockInfo,
//         string typeId,
//         out TransactionInfo transactionInfo,
//         out int actionIndex)
//     {
//         for (var i = 0; i < blockInfo.Transactions.Length; i++)
//         {
//             var transaction = blockInfo.Transactions[i];
//             for (var j = 0; j < transaction.Actions.Length; j++)
//             {
//                 var action = transaction.Actions[j];
//                 if (action.TypeId == typeId)
//                 {
//                     transactionInfo = transaction;
//                     actionIndex = j;
//                     return true;
//                 }
//             }
//         }

//         transactionInfo = default;
//         actionIndex = -1;
//         return false;
//     }

//     private async void Client_BlockAppended(object? sender, BlockEventArgs e)
//     {
//         var blockInfo = e.BlockInfo;

//         foreach (var (typeId, handler) in _actionHandlers)
//         {
//             if (ContainsAction(blockInfo, typeId, out var transactionInfo, out var actionIndex))
//             {
//                 await handler(transactionInfo, actionIndex);
//             }
//         }
//     }

//     private async Task OnGuildCreatedAsync(TransactionInfo transactionInfo, int actionIndex)
//     {
//         if (transactionInfo.Signer != _client.Address)
//         {
//             await Console.Out.WriteLineAsync($"Guild created: {transactionInfo.Signer}");
//         }
//     }

//     private async Task OnGuildDeletedAsync(TransactionInfo transactionInfo, int actionIndex)
//     {
//         if (transactionInfo.Signer != _client.Address)
//         {
//             await Console.Out.WriteLineAsync($"Guild deleted: {transactionInfo.Signer}");
//         }
//     }

//     private async Task OnGuildJoinRequestedAsync(TransactionInfo transactionInfo, int actionIndex)
//     {
//         if (transactionInfo.Signer != _client.Address)
//         {
//             var blockChain = _blockChain;
//             var bytes = await blockChain.GetActionAsync(transactionInfo.Id, actionIndex, default);
//             var value = _codec.Decode(bytes);
//             var action = new ApplyGuild();
//             action.LoadPlainValue(value);
//             await Console.Out.WriteLineAsync(
//                 $"Guild join requested: {transactionInfo.Signer} {action.GuildAddress}");
//         }
//     }

//     private async Task OnGuildJoinCancelledAsync(TransactionInfo transactionInfo, int actionIndex)
//     {
//         if (transactionInfo.Signer != _client.Address)
//         {
//             await Console.Out.WriteLineAsync($"Guild join cancelled: {transactionInfo.Signer}");
//         }
//     }

//     private async Task OnGuildJoinAcceptedAsync(TransactionInfo transactionInfo, int actionIndex)
//     {
//         if (transactionInfo.Signer != _client.Address)
//         {
//             var blockChain = _blockChain;
//             var bytes = await blockChain.GetActionAsync(transactionInfo.Id, actionIndex, default);
//             var value = _codec.Decode(bytes);
//             var action = new AcceptGuildApplication();
//             action.LoadPlainValue(value);
//             await Console.Out.WriteLineAsync(
//                 $"Guild join accepted: {transactionInfo.Signer} {action.Target}");
//         }
//     }

//     private async Task OnGuildJoinRejectedAsync(TransactionInfo transactionInfo, int actionIndex)
//     {
//         if (transactionInfo.Signer != _client.Address)
//         {
//             var blockChain = _blockChain;
//             var bytes = await blockChain.GetActionAsync(transactionInfo.Id, actionIndex, default);
//             var value = _codec.Decode(bytes);
//             var action = new RejectGuildApplication();
//             action.LoadPlainValue(value);
//             await Console.Out.WriteLineAsync(
//                 $"Guild join rejected: {transactionInfo.Signer} {action.Target}");
//         }
//     }

//     private async Task OnGuildMemberBannedAsync(TransactionInfo transactionInfo, int actionIndex)
//     {
//         if (transactionInfo.Signer != _client.Address)
//         {
//             var blockChain = _blockChain;
//             var bytes = await blockChain.GetActionAsync(transactionInfo.Id, actionIndex, default);
//             var value = _codec.Decode(bytes);
//             var action = new BanGuildMember();
//             action.LoadPlainValue(value);
//             await Console.Out.WriteLineAsync(
//                 $"Guild member banned: {transactionInfo.Signer} {action.Target}");
//         }
//     }

//     private async Task OnGuildMemberUnbannedAsync(TransactionInfo transactionInfo, int actionIndex)
//     {
//         if (transactionInfo.Signer != _client.Address)
//         {
//             var blockChain = _blockChain;
//             var bytes = await blockChain.GetActionAsync(transactionInfo.Id, actionIndex, default);
//             var value = _codec.Decode(bytes);
//             var action = new UnbanGuildMember();
//             action.LoadPlainValue(value);
//             await Console.Out.WriteLineAsync(
//                 $"Guild member unbanned: {transactionInfo.Signer} {action.Target}");
//         }
//     }
// }

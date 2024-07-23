using System.ComponentModel.Composition;
using Bencodex;
using Libplanet.Crypto;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Frameworks;
using LibplanetConsole.Guild;
using LibplanetConsole.Nodes;
using Nekoyume.Action.Guild;

namespace LibplanetConsole.Clients.Guild;

[Export(typeof(IApplicationService))]
internal sealed class GuildEventTracer : IApplicationService
{
    private static readonly Codec _codec = new();

    private readonly Dictionary<string, Func<TransactionInfo, int, Task>> _actionHandlers;
    private readonly IClient _client;
    private readonly IBlockChain _blockChain;

    [ImportingConstructor]
    public GuildEventTracer(IClient client, IBlockChain blockChain)
    {
        _client = client;
        _blockChain = blockChain;
        _actionHandlers = new()
        {
            { MakeGuild.TypeIdentifier, OnGuildCreatedAsync },
            { RemoveGuild.TypeIdentifier, OnGuildDeletedAsync },
            { ApplyGuild.TypeIdentifier, OnGuildJoinRequestedAsync },
            { CancelGuildApplication.TypeIdentifier, OnGuildJoinCancelledAsync },
            { AcceptGuildApplication.TypeIdentifier, OnGuildJoinAcceptedAsync },
            { RejectGuildApplication.TypeIdentifier, OnGuildJoinRejectedAsync },
            { BanGuildMember.TypeIdentifier, OnGuildMemberBannedAsync },
            { UnbanGuildMember.TypeIdentifier, OnGuildMemberUnbannedAsync },
        };
    }

    public Task InitializeAsync(
        IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        _client.BlockAppended += Client_BlockAppended;
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        _client.BlockAppended -= Client_BlockAppended;
        return ValueTask.CompletedTask;
    }

    private static bool ContainsAction(
        BlockInfo blockInfo,
        string typeId,
        out TransactionInfo transactionInfo,
        out int actionIndex)
    {
        for (var i = 0; i < blockInfo.Transactions.Length; i++)
        {
            var transaction = blockInfo.Transactions[i];
            if (transaction.IsFailed == true)
            {
                continue;
            }

            for (var j = 0; j < transaction.Actions.Length; j++)
            {
                var action = transaction.Actions[j];
                if (action.TypeId == typeId)
                {
                    transactionInfo = transaction;
                    actionIndex = j;
                    return true;
                }
            }
        }

        transactionInfo = default;
        actionIndex = -1;
        return false;
    }

    private async void Client_BlockAppended(object? sender, BlockEventArgs e)
    {
        var blockInfo = e.BlockInfo;

        foreach (var (typeId, handler) in _actionHandlers)
        {
            if (ContainsAction(blockInfo, typeId, out var transactionInfo, out var actionIndex))
            {
                await handler(transactionInfo, actionIndex);
            }
        }
    }

    private async Task OnGuildCreatedAsync(TransactionInfo transactionInfo, int actionIndex)
    {
        if (transactionInfo.Signer != _client.Address)
        {
            var guildAddress = transactionInfo.Signer;
            var message = GuildEventMessage.CreatedMessage(guildAddress);
            await Console.Out.WriteLineAsJsonAsync(message);
        }
    }

    private async Task OnGuildDeletedAsync(TransactionInfo transactionInfo, int actionIndex)
    {
        if (transactionInfo.Signer != _client.Address)
        {
            var guildMessage = transactionInfo.Signer;
            var message = GuildEventMessage.DeletedMessage(guildMessage);
            await Console.Out.WriteLineAsync(message);
        }
    }

    private async Task OnGuildJoinRequestedAsync(TransactionInfo transactionInfo, int actionIndex)
    {
        if (transactionInfo.Signer != _client.Address)
        {
            var blockChain = _blockChain;
            var bytes = await blockChain.GetActionAsync(transactionInfo.Id, actionIndex, default);
            var value = _codec.Decode(bytes);
            var action = new ApplyGuild();
            action.LoadPlainValue(value);

            var guildAddress = (AppAddress)(Address)action.GuildAddress;
            var memberAddress = transactionInfo.Signer;
            var message = GuildEventMessage.RequestedJoinMessage(guildAddress, memberAddress);
            await Console.Out.WriteLineAsync(message);
        }
    }

    private async Task OnGuildJoinCancelledAsync(TransactionInfo transactionInfo, int actionIndex)
    {
        if (transactionInfo.Signer != _client.Address)
        {
            var memberAddress = transactionInfo.Signer;
            var message = GuildEventMessage.CancelledJoinMessage(memberAddress);
            await Console.Out.WriteLineAsync(message);
        }
    }

    private async Task OnGuildJoinAcceptedAsync(TransactionInfo transactionInfo, int actionIndex)
    {
        if (transactionInfo.Signer != _client.Address)
        {
            var blockChain = _blockChain;
            var bytes = await blockChain.GetActionAsync(transactionInfo.Id, actionIndex, default);
            var value = _codec.Decode(bytes);
            var action = new AcceptGuildApplication();
            action.LoadPlainValue(value);

            var guildAddress = transactionInfo.Signer;
            var memberAddress = (AppAddress)(Address)action.Target;
            var message = GuildEventMessage.AcceptedJoinMessage(guildAddress, memberAddress);
            await Console.Out.WriteLineAsync(message);
        }
    }

    private async Task OnGuildJoinRejectedAsync(TransactionInfo transactionInfo, int actionIndex)
    {
        if (transactionInfo.Signer != _client.Address)
        {
            var blockChain = _blockChain;
            var bytes = await blockChain.GetActionAsync(transactionInfo.Id, actionIndex, default);
            var value = _codec.Decode(bytes);
            var action = new RejectGuildApplication();
            action.LoadPlainValue(value);

            var guildAddress = transactionInfo.Signer;
            var memberAddress = (AppAddress)(Address)action.Target;
            var message = GuildEventMessage.RejectedJoinMessage(guildAddress, memberAddress);
            await Console.Out.WriteLineAsync(message);
        }
    }

    private async Task OnGuildMemberBannedAsync(TransactionInfo transactionInfo, int actionIndex)
    {
        if (transactionInfo.Signer != _client.Address)
        {
            var blockChain = _blockChain;
            var bytes = await blockChain.GetActionAsync(transactionInfo.Id, actionIndex, default);
            var value = _codec.Decode(bytes);
            var action = new BanGuildMember();
            action.LoadPlainValue(value);

            var guildAddress = transactionInfo.Signer;
            var memberAddress = (AppAddress)(Address)action.Target;
            var message = GuildEventMessage.BannedMessage(guildAddress, memberAddress);
            await Console.Out.WriteLineAsync(message);
        }
    }

    private async Task OnGuildMemberUnbannedAsync(TransactionInfo transactionInfo, int actionIndex)
    {
        if (transactionInfo.Signer != _client.Address)
        {
            var blockChain = _blockChain;
            var bytes = await blockChain.GetActionAsync(transactionInfo.Id, actionIndex, default);
            var value = _codec.Decode(bytes);
            var action = new UnbanGuildMember();
            action.LoadPlainValue(value);

            var guildAddress = transactionInfo.Signer;
            var memberAddress = (AppAddress)action.Target;
            var message = GuildEventMessage.UnbannedMessage(guildAddress, memberAddress);
            await Console.Out.WriteLineAsync(message);
        }
    }
}

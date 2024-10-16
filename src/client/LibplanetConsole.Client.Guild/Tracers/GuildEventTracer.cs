using Libplanet.Crypto;
using LibplanetConsole.Common;
using LibplanetConsole.Frameworks;
using LibplanetConsole.Guild;
using LibplanetConsole.Nodes;
using Nekoyume.Action.Guild;

namespace LibplanetConsole.Client.Guild.Tracers;

[Export(typeof(IApplicationService))]
internal sealed class GuildEventTracer : IApplicationService, IDisposable
{
    private readonly Dictionary<string, Func<TransactionInfo, ActionInfo, CancellationToken, Task>>
        _actionHandlers;

    private readonly IClient _client;
    private readonly IBlockChain _blockChain;
    private readonly IGuildClient _guildClient;

    [ImportingConstructor]
    public GuildEventTracer(IClient client, IBlockChain blockChain, IGuildClient guildClient)
    {
        _client = client;
        _blockChain = blockChain;
        _guildClient = guildClient;
        _actionHandlers = new()
        {
            { MakeGuild.TypeIdentifier, OnGuildCreatedAsync },
            { RemoveGuild.TypeIdentifier, OnGuildDeletedAsync },
            { ApplyGuild.TypeIdentifier, OnGuildJoinRequestedAsync },
            { CancelGuildApplication.TypeIdentifier, OnGuildJoinCanceledAsync },
            { AcceptGuildApplication.TypeIdentifier, OnGuildJoinAcceptedAsync },
            { RejectGuildApplication.TypeIdentifier, OnGuildJoinRejectedAsync },
            { BanGuildMember.TypeIdentifier, OnGuildMemberBannedAsync },
            { UnbanGuildMember.TypeIdentifier, OnGuildMemberUnbannedAsync },
        };
    }

    public Task InitializeAsync(
        IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        _blockChain.BlockAppended += BlockChain_BlockAppended;
        return Task.CompletedTask;
    }

    void IDisposable.Dispose()
    {
        _blockChain.BlockAppended -= BlockChain_BlockAppended;
    }

    private static bool ContainsAction(
        BlockInfo blockInfo,
        string typeId,
        out TransactionInfo transactionInfo,
        out ActionInfo actionInfo)
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
                    actionInfo = action;
                    return true;
                }
            }
        }

        transactionInfo = default;
        actionInfo = default;
        return false;
    }

    private async void BlockChain_BlockAppended(object? sender, BlockEventArgs e)
    {
        var blockInfo = e.BlockInfo;

        foreach (var (typeId, handler) in _actionHandlers)
        {
            if (ContainsAction(blockInfo, typeId, out var transactionInfo, out var actionIndex))
            {
                await handler(transactionInfo, actionIndex, default);
            }
        }
    }

    private async Task OnGuildCreatedAsync(
        TransactionInfo transactionInfo, ActionInfo actionInfo, CancellationToken cancellationToken)
    {
        if (transactionInfo.Signer != _client.Address)
        {
            var address = transactionInfo.Signer;
            var guildAddress = await _guildClient.GetGuildAsync(address, cancellationToken);
            var message = GuildEventMessage.CreatedMessage(guildAddress);
            await Console.Out.WriteLineAsync(message);
        }
    }

    private async Task OnGuildDeletedAsync(
        TransactionInfo transactionInfo, ActionInfo actionInfo, CancellationToken cancellationToken)
    {
        if (transactionInfo.Signer != _client.Address)
        {
            var height = transactionInfo.Height - 1;
            var address = transactionInfo.Signer;
            var guildMessage = await _guildClient.GetGuildAsync(height, address, cancellationToken);
            var message = GuildEventMessage.DeletedMessage(guildMessage);
            await Console.Out.WriteLineAsync(message);
        }
    }

    private async Task OnGuildJoinRequestedAsync(
        TransactionInfo transactionInfo, ActionInfo actionInfo, CancellationToken cancellationToken)
    {
        if (transactionInfo.Signer != _client.Address)
        {
            var blockChain = _blockChain;
            var action = await blockChain.GetActionAsync<ApplyGuild>(
                transactionInfo.Id, actionInfo.Index, cancellationToken);
            var guildAddress = (AppAddress)(Address)action.GuildAddress;
            var memberAddress = transactionInfo.Signer;
            var message = GuildEventMessage.RequestedJoinMessage(guildAddress, memberAddress);
            await Console.Out.WriteLineAsync(message);
        }
    }

    private async Task OnGuildJoinCanceledAsync(
        TransactionInfo transactionInfo, ActionInfo actionInfo, CancellationToken cancellationToken)
    {
        if (transactionInfo.Signer != _client.Address)
        {
            var memberAddress = transactionInfo.Signer;
            var message = GuildEventMessage.CanceledJoinMessage(memberAddress);
            await Console.Out.WriteLineAsync(message);
        }
    }

    private async Task OnGuildJoinAcceptedAsync(
        TransactionInfo transactionInfo, ActionInfo actionInfo, CancellationToken cancellationToken)
    {
        if (transactionInfo.Signer != _client.Address)
        {
            var blockChain = _blockChain;
            var action = await blockChain.GetActionAsync<AcceptGuildApplication>(
                transactionInfo.Id, actionInfo.Index, cancellationToken);
            var guildAddress = await _guildClient.GetGuildAsync(
                transactionInfo.Signer, cancellationToken);
            var memberAddress = (AppAddress)(Address)action.Target;
            var message = GuildEventMessage.AcceptedJoinMessage(guildAddress, memberAddress);
            await Console.Out.WriteLineAsync(message);
        }
    }

    private async Task OnGuildJoinRejectedAsync(
        TransactionInfo transactionInfo, ActionInfo actionInfo, CancellationToken cancellationToken)
    {
        if (transactionInfo.Signer != _client.Address)
        {
            var blockChain = _blockChain;
            var action = await blockChain.GetActionAsync<RejectGuildApplication>(
                transactionInfo.Id, actionInfo.Index, cancellationToken);
            var guildAddress = await _guildClient.GetGuildAsync(
                transactionInfo.Signer, cancellationToken);
            var memberAddress = (AppAddress)(Address)action.Target;
            var message = GuildEventMessage.RejectedJoinMessage(guildAddress, memberAddress);
            await Console.Out.WriteLineAsync(message);
        }
    }

    private async Task OnGuildMemberBannedAsync(
        TransactionInfo transactionInfo, ActionInfo actionInfo, CancellationToken cancellationToken)
    {
        if (transactionInfo.Signer != _client.Address)
        {
            var blockChain = _blockChain;
            var action = await blockChain.GetActionAsync<BanGuildMember>(
                transactionInfo.Id, actionInfo.Index, cancellationToken);
            var guildAddress = await _guildClient.GetGuildAsync(
                transactionInfo.Signer, cancellationToken);
            var memberAddress = (AppAddress)(Address)action.Target;
            var message = GuildEventMessage.BannedMessage(guildAddress, memberAddress);
            await Console.Out.WriteLineAsync(message);
        }
    }

    private async Task OnGuildMemberUnbannedAsync(
        TransactionInfo transactionInfo, ActionInfo actionInfo, CancellationToken cancellationToken)
    {
        if (transactionInfo.Signer != _client.Address)
        {
            var blockChain = _blockChain;
            var action = await blockChain.GetActionAsync<UnbanGuildMember>(
                transactionInfo.Id, actionInfo.Index, cancellationToken);
            var guildAddress = await _guildClient.GetGuildAsync(
                transactionInfo.Signer, cancellationToken);
            var memberAddress = (AppAddress)action.Target;
            var message = GuildEventMessage.UnbannedMessage(guildAddress, memberAddress);
            await Console.Out.WriteLineAsync(message);
        }
    }
}

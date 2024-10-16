using System.Text;
using Bencodex.Types;
using Libplanet.Action.State;
using Libplanet.Blockchain;
using Libplanet.Crypto;
using Libplanet.Types.Blocks;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Guild;
using Microsoft.Extensions.DependencyInjection;
using Nekoyume;
using Nekoyume.Action.Guild;
using Nekoyume.Model.Guild;
using Nekoyume.Module.Guild;
using Nekoyume.TypedAddress;

namespace LibplanetConsole.Node.Guild;

internal sealed class Guild : IGuild, IDisposable
{
    private readonly INode _node;
    private readonly IBlockChain _blockChain;

    public Guild(INode node, IBlockChain blockChain)
    {
        _node = node;
        _blockChain = blockChain;
        _node.Started += Node_Started;
        _node.Stopped += Node_Stopped;
    }

    public GuildInfo Info { get; private set; }

    public async Task CreateAsync(CreateGuildOptions options, CancellationToken cancellationToken)
    {
        ThrowIfNotRunning();

        var makeGuild = new MakeGuild
        {
        };
        await _blockChain.AddTransactionAsync([makeGuild], cancellationToken);
        Info = GetGuildInfo();
    }

    public async Task<Address> DeleteAsync(
        DeleteGuildOptions options, CancellationToken cancellationToken)
    {
        ThrowIfNotRunning();

        var removeGuild = new RemoveGuild
        {
        };
        var guildAddress = Info.Address;
        await _blockChain.AddTransactionAsync([removeGuild], cancellationToken);
        Info = default;
        return guildAddress;
    }

    public async Task RequestJoinAsync(
        RequestJoinOptions options, CancellationToken cancellationToken)
    {
        ThrowIfNotRunning();

        var action = new ApplyGuild(new GuildAddress((Address)options.GuildAddress))
        {
        };
        await _blockChain.AddTransactionAsync([action], cancellationToken);
    }

    public async Task CancelJoinAsync(
        CancelJoinOptions options, CancellationToken cancellationToken)
    {
        ThrowIfNotRunning();

        var action = new CancelGuildApplication
        {
        };
        await _blockChain.AddTransactionAsync([action], cancellationToken);
    }

    public async Task AcceptJoinAsync(
        AcceptJoinOptions options, CancellationToken cancellationToken)
    {
        ThrowIfNotRunning();

        var action = new AcceptGuildApplication(new AgentAddress((Address)options.MemberAddress))
        {
        };
        await _blockChain.AddTransactionAsync([action], cancellationToken);
    }

    public async Task RejectJoinAsync(
        RejectJoinOptions options, CancellationToken cancellationToken)
    {
        ThrowIfNotRunning();

        var action = new RejectGuildApplication(new AgentAddress((Address)options.MemberAddress))
        {
        };
        await _blockChain.AddTransactionAsync([action], cancellationToken);
    }

    public async Task QuitAsync(LeaveGuildOptions options, CancellationToken cancellationToken)
    {
        ThrowIfNotRunning();

        var quitGuild = new QuitGuild
        {
        };
        await _blockChain.AddTransactionAsync([quitGuild], cancellationToken);
    }

    public async Task BanMemberAsync(BanMemberOptions options, CancellationToken cancellationToken)
    {
        ThrowIfNotRunning();

        var memberAddress = (Address)options.MemberAddress;
        var banGuildMember = new BanGuildMember(new(memberAddress))
        {
        };
        await _blockChain.AddTransactionAsync([banGuildMember], cancellationToken);
    }

    public async Task UnbanMemberAsync(
        UnbanMemberOptions options, CancellationToken cancellationToken)
    {
        ThrowIfNotRunning();

        var memberAddress = (Address)options.MemberAddress;
        var unbanMemberGuild = new UnbanGuildMember(memberAddress)
        {
        };
        await _blockChain.AddTransactionAsync([unbanMemberGuild], cancellationToken);
    }

    public Task<Address> GetGuildAsync(
        long height, Address address, CancellationToken cancellationToken)
    {
        Address GetGuild()
        {
            var blockChain = _node.GetRequiredService<BlockChain>();
            var block = height == long.MaxValue ? blockChain.Tip : blockChain[height];
            var worldState = GetWorldState(blockChain, block);
            var agentAddress = new AgentAddress((Address)address);
            if (GuildParticipantModule.GetJoinedGuild(worldState, agentAddress) is { } guildAddress)
            {
                return (Address)(Address)guildAddress;
            }

            return default;
        }

        return Task.Run(GetGuild, cancellationToken);
    }

    public async Task<Address[]> GetGuildMembersAsync(
        long height, Address guildAddress, CancellationToken cancellationToken)
    {
        Address[] GetGuildMembers()
        {
            var blockChain = _node.GetRequiredService<BlockChain>();
            var block = height == long.MaxValue ? blockChain.Tip : blockChain[height];
            var worldState = GetWorldState(blockChain, block);
            var trie = worldState.GetAccountState(Addresses.GuildParticipant).Trie;
            IEnumerable<(Address, GuildParticipant)> guildParticipants = trie.IterateValues()
                .Where(pair => pair.Value is List)
                .Select(pair => (
                    new Address(Encoding.UTF8.GetString(Convert.FromHexString(pair.Path.Hex))),
                    new GuildParticipant((List)pair.Value)));
            var filtered = guildParticipants
                .Where(pair => pair.Item2.GuildAddress == new GuildAddress((Address)guildAddress));
            return [.. filtered.Select(item => (Address)item.Item1)];
        }

        return await Task.Run(GetGuildMembers, cancellationToken);
    }

    public void Dispose()
    {
        _node.Started -= Node_Started;
        _node.Stopped -= Node_Stopped;
    }

    private static IWorldState GetWorldState(BlockChain blockChain, Block block)
    {
        if (blockChain.Tip == block)
        {
            return blockChain.GetNextWorldState() ?? blockChain.GetWorldState();
        }

        return blockChain.GetWorldState(block.Hash);
    }

    private GuildInfo GetGuildInfo()
    {
        var blockChain = _node.GetRequiredService<BlockChain>();
        var worldState = blockChain.GetNextWorldState() ?? blockChain.GetWorldState();
        var agentAddress = new AgentAddress((Address)_node.Address);
        if (GuildParticipantModule.GetJoinedGuild(worldState, agentAddress) is { } guildAddress)
        {
            return new()
            {
                Address = (Address)(Address)guildAddress,
            };
        }

        return default;
    }

    private void Node_Started(object? sender, EventArgs e) => Info = GetGuildInfo();

    private void Node_Stopped(object? sender, EventArgs e) => Info = default;

    private void ThrowIfNotRunning()
    {
        if (_node.IsRunning != true)
        {
            throw new InvalidOperationException("The guild is not running.");
        }
    }
}

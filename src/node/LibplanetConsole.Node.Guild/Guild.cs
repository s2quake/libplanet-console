using LibplanetConsole.Guild;
using Nekoyume;
using Nekoyume.Action.Guild;

namespace LibplanetConsole.Node.Guild;

internal sealed class Guild(IBlockChain blockChain)
    : NodeContentBase(nameof(Guild)), IGuild
{
    public bool IsRunning { get; private set; }

    public GuildInfo Info { get; private set; }

    public async Task CreateAsync(CreateGuildOptions options, CancellationToken cancellationToken)
    {
        ThrowIfNotRunning();

        var makeGuild = new MakeGuild
        {
        };
        await blockChain.SendTransactionAsync([makeGuild], cancellationToken);
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
        await blockChain.SendTransactionAsync([removeGuild], cancellationToken);
        Info = default;
        return guildAddress;
    }

    public async Task QuitAsync(LeaveGuildOptions options, CancellationToken cancellationToken)
    {
        ThrowIfNotRunning();

        var quitGuild = new QuitGuild
        {
        };
        await blockChain.SendTransactionAsync([quitGuild], cancellationToken);
    }

    public async Task BanMemberAsync(BanMemberOptions options, CancellationToken cancellationToken)
    {
        ThrowIfNotRunning();

        var memberAddress = options.MemberAddress;
        var banGuildMember = new BanGuildMember(new(memberAddress))
        {
        };
        await blockChain.SendTransactionAsync([banGuildMember], cancellationToken);
    }

    public async Task UnbanMemberAsync(
        UnbanMemberOptions options, CancellationToken cancellationToken)
    {
        ThrowIfNotRunning();

        var memberAddress = options.MemberAddress;
        var unbanMemberGuild = new UnbanGuildMember(memberAddress)
        {
        };
        await blockChain.SendTransactionAsync([unbanMemberGuild], cancellationToken);
    }

    public Task<Address> GetGuildAsync(
        long height, Address address, CancellationToken cancellationToken)
    {
        Address GetGuild()
        {
            // var block = height == long.MaxValue ? blockChain.Tip : blockChain[height];
            // var worldState = GetWorldState(blockChain, block);
            // var agentAddress = new AgentAddress(address);
            // if (GuildParticipantModule.GetJoinedGuild(worldState, agentAddress) is { } guildAddress)
            // {
            //     return guildAddress;
            // }

            return default;
        }

        return Task.Run(GetGuild, cancellationToken);
    }

    public async Task<Address[]> GetGuildMembersAsync(
        long height, Address guildAddress, CancellationToken cancellationToken)
    {
        Address[] GetGuildMembers()
        {
            // var blockChain = node.GetRequiredService<BlockChain>();
            // var block = height == long.MaxValue ? blockChain.Tip : blockChain[height];
            // var worldState = GetWorldState(blockChain, block);
            // var trie = worldState.GetAccountState(Addresses.GuildParticipant).Trie;
            // IEnumerable<(Address, GuildParticipant)> guildParticipants = trie.IterateValues()
            //     .Where(pair => pair.Value is List)
            //     .Select(pair => (
            //         new Address(Encoding.UTF8.GetString(Convert.FromHexString(pair.Path.Hex))),
            //         new GuildParticipant((List)pair.Value)));
            // var filtered = guildParticipants
            //     .Where(pair => pair.Item2.GuildAddress == new GuildAddress(guildAddress));
            // return [.. filtered.Select(item => item.Item1)];
            throw new NotImplementedException();
        }

        return await Task.Run(GetGuildMembers, cancellationToken);
    }

    private GuildInfo GetGuildInfo()
    {
        // var blockChain = node.GetRequiredService<BlockChain>();
        // var worldState = blockChain.GetNextWorldState() ?? blockChain.GetWorldState();
        // var agentAddress = new AgentAddress(node.Address);
        // if (GuildParticipantModule.GetJoinedGuild(worldState, agentAddress) is { } guildAddress)
        // {
        //     return new()
        //     {
        //         Address = guildAddress,
        //     };
        // }

        return default;
    }

    private void ThrowIfNotRunning()
    {
        if (IsRunning != true)
        {
            throw new InvalidOperationException("The guild is not running.");
        }
    }

    protected override Task OnStartAsync(CancellationToken cancellationToken)
    {
        IsRunning = true;
        blockChain.GetWorldState().GetAccountState(Addresses.Guild);
        return Task.CompletedTask;
    }

    protected override Task OnStopAsync(CancellationToken cancellationToken)
    {
        IsRunning = false;
        return Task.CompletedTask;
    }
}

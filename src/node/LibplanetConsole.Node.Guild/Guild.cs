using Libplanet.Action.State;
using Nekoyume.Action.Guild;
using Nekoyume.Model.Guild;
using Nekoyume.TypedAddress;

namespace LibplanetConsole.Node.Guild;

internal sealed class Guild(INode node, IBlockChain blockChain)
    : NodeContentBase(nameof(Guild)), IGuild
{
    public bool IsRunning { get; private set; }

    public GuildInfo Info { get; private set; }

    public async Task<GuildInfo> CreateAsync(CancellationToken cancellationToken)
    {
        ThrowIfNotRunning();

        var makeGuild = new MakeGuild(node.Address)
        {
        };
        await node.SendTransactionAsync([makeGuild], cancellationToken);
        Info = GetGuildInfo();
        return Info;
    }

    public async Task<Address> DeleteAsync(CancellationToken cancellationToken)
    {
        ThrowIfNotRunning();

        var removeGuild = new RemoveGuild
        {
        };
        var guildAddress = Info.Address;
        await node.SendTransactionAsync([removeGuild], cancellationToken);
        Info = default;
        return guildAddress;
    }

    public async Task JoinAsync(Address guildAddress, CancellationToken cancellationToken)
    {
        ThrowIfNotRunning();

        var joinGuild = new JoinGuild(new(guildAddress))
        {
        };
        await node.SendTransactionAsync([joinGuild], cancellationToken);
    }

    public async Task LeaveAsync(CancellationToken cancellationToken)
    {
        ThrowIfNotRunning();

        var quitGuild = new QuitGuild
        {
        };
        await node.SendTransactionAsync([quitGuild], cancellationToken);
    }

    public async Task BanMemberAsync(Address memberAddress, CancellationToken cancellationToken)
    {
        ThrowIfNotRunning();

        var banGuildMember = new BanGuildMember(new(memberAddress))
        {
        };
        await node.SendTransactionAsync([banGuildMember], cancellationToken);
    }

    public async Task UnbanMemberAsync(
        Address memberAddress, CancellationToken cancellationToken)
    {
        ThrowIfNotRunning();

        var unbanMemberGuild = new UnbanGuildMember(memberAddress)
        {
        };
        await node.SendTransactionAsync([unbanMemberGuild], cancellationToken);
    }

    public Task<GuildInfo> GetGuildAsync(Address address, CancellationToken cancellationToken)
        => Task.Run(GetGuildInfo);

    protected override Task OnStartAsync(CancellationToken cancellationToken)
    {
        IsRunning = true;
        return Task.CompletedTask;
    }

    protected override Task OnStopAsync(CancellationToken cancellationToken)
    {
        IsRunning = false;
        return Task.CompletedTask;
    }

    private GuildInfo GetGuildInfo()
    {
        var nodeAddress = node.Address;
        var worldState = blockChain.GetWorldState();
        var world = new World(worldState);
        var guildRepository = new GuildRepository(world, new ActionContext());
        var agentAddress = new AgentAddress(nodeAddress);
        var guildParticipant = guildRepository.GetGuildParticipant(agentAddress);
        var guild = guildRepository.GetGuild(guildParticipant.GuildAddress);
        return new GuildInfo
        {
            Address = guild.Address,
            ValidatorAddress = guild.ValidatorAddress,
            GuildMasterAddress = guild.GuildMasterAddress,
        };
    }

    private void ThrowIfNotRunning()
    {
        if (IsRunning != true)
        {
            throw new InvalidOperationException("The guild is not running.");
        }
    }
}

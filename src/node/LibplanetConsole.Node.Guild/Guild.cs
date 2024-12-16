using Libplanet.Action.State;
using LibplanetConsole.Node.Bank;
using Nekoyume.Action.Guild;
using Nekoyume.Model.Guild;
using Nekoyume.Model.State;
using Nekoyume.TypedAddress;

namespace LibplanetConsole.Node.Guild;

internal sealed class Guild(INode node, IBlockChain blockChain)
    : NodeContentBase(nameof(Guild)), IGuild, ICurrencyProvider
{
    private Currency? _goldCurrency;

    public bool IsRunning { get; private set; }

    IEnumerable<CurrencyInfo> ICurrencyProvider.Currencies
    {
        get
        {
            if (_goldCurrency is { } goldCurrency)
            {
                yield return new CurrencyInfo
                {
                    Code = "ncg",
                    Currency = goldCurrency,
                };
            }
        }
    }

    public async Task CreateAsync(CancellationToken cancellationToken)
    {
        ThrowIfNotRunning();

        var makeGuild = new MakeGuild(node.Address)
        {
        };
        await node.SendTransactionAsync([makeGuild], cancellationToken);
    }

    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
        ThrowIfNotRunning();

        var removeGuild = new RemoveGuild
        {
        };
        await node.SendTransactionAsync([removeGuild], cancellationToken);
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

    public async Task MoveAsync(Address guildAddress, CancellationToken cancellationToken)
    {
        ThrowIfNotRunning();

        var moveGuild = new MoveGuild(new(guildAddress))
        {
        };
        await node.SendTransactionAsync([moveGuild], cancellationToken);
    }

    public async Task BanAsync(Address memberAddress, CancellationToken cancellationToken)
    {
        ThrowIfNotRunning();

        var banGuildMember = new BanGuildMember(new(memberAddress))
        {
        };
        await node.SendTransactionAsync([banGuildMember], cancellationToken);
    }

    public async Task UnbanAsync(
        Address memberAddress, CancellationToken cancellationToken)
    {
        ThrowIfNotRunning();

        var unbanMemberGuild = new UnbanGuildMember(memberAddress)
        {
        };
        await node.SendTransactionAsync([unbanMemberGuild], cancellationToken);
    }

    public async Task ClaimAsync(CancellationToken cancellationToken)
    {
        ThrowIfNotRunning();

        var claimReward = new ClaimReward
        {
        };
        await node.SendTransactionAsync([claimReward], cancellationToken);
    }

    public Task<GuildInfo> GetInfoAsync(Address memberAddress, CancellationToken cancellationToken)
    {
        var info = GetGuildInfo(memberAddress);
        return Task.FromResult(info);
    }

    protected override async Task OnStartAsync(CancellationToken cancellationToken)
    {
        var worldStae = blockChain.GetWorldState();
        var accountState = worldStae.GetAccountState(ReservedAddresses.LegacyAccount);
        var value = accountState.GetState(GoldCurrencyState.Address);
        if (value is not Dictionary dictionary)
        {
            throw new InvalidOperationException(
                "The states doesn't contain gold currency.\n" +
                "Check the genesis block.");
        }

        _goldCurrency = new GoldCurrencyState(dictionary).Currency;
        IsRunning = true;
        await Task.CompletedTask;
    }

    protected override Task OnStopAsync(CancellationToken cancellationToken)
    {
        IsRunning = false;
        return Task.CompletedTask;
    }

    private GuildInfo GetGuildInfo(Address memberAddress)
    {
        var worldState = blockChain.GetWorldState();
        var world = new World(worldState);
        var guildRepository = new GuildRepository(world, new ActionContext());
        var agentAddress = new AgentAddress(memberAddress);
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

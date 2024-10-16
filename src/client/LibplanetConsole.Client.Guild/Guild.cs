using Libplanet.Crypto;
using LibplanetConsole.Client.Guild.Services;
using LibplanetConsole.Common;
using LibplanetConsole.Guild;
using Nekoyume.Action.Guild;
using Nekoyume.TypedAddress;

namespace LibplanetConsole.Client.Guild;

internal sealed class Guild : IGuildClient, IDisposable
{
    private readonly IClient _client;
    private readonly IBlockChain _blockChain;
    private readonly RemoteGuildService _remoteGuildService;
    private bool _isRunning;

    public Guild(
        IClient client, IBlockChain blockChain, RemoteGuildService remoteGuildService)
    {
        _client = client;
        _blockChain = blockChain;
        _remoteGuildService = remoteGuildService;
        _client.Started += Client_Started;
        _client.Stopped += Client_Stopped;
    }

    public GuildInfo Info { get; private set; }

    public async Task CreateAsync(CreateGuildOptions options, CancellationToken cancellationToken)
    {
        ThrowIfNotRunning();

        var makeGuild = new MakeGuild
        {
        };
        await _blockChain.SendTransactionAsync([makeGuild], cancellationToken);
        var guildAddress = await _remoteGuildService.Service.GetGuildAsync(
            long.MaxValue, _client.Address, cancellationToken);
        Info = Info with { Address = guildAddress };
    }

    public async Task<Address> DeleteAsync(
        DeleteGuildOptions options, CancellationToken cancellationToken)
    {
        ThrowIfNotRunning();

        var removeGuild = new RemoveGuild
        {
        };
        var guildAddress = Info.Address;
        await _blockChain.SendTransactionAsync([removeGuild], cancellationToken);
        Info = Info with { Address = default, };
        return guildAddress;
    }

    public async Task RequestJoinAsync(
        RequestJoinOptions options, CancellationToken cancellationToken)
    {
        ThrowIfNotRunning();

        var action = new ApplyGuild(new GuildAddress((Address)options.GuildAddress))
        {
        };
        await _blockChain.SendTransactionAsync([action], cancellationToken);
    }

    public async Task CancelJoinAsync(
        CancelJoinOptions options, CancellationToken cancellationToken)
    {
        ThrowIfNotRunning();

        var action = new CancelGuildApplication()
        {
        };
        await _blockChain.SendTransactionAsync([action], cancellationToken);
    }

    public async Task AcceptJoinAsync(
        AcceptJoinOptions options, CancellationToken cancellationToken)
    {
        ThrowIfNotRunning();

        var action = new AcceptGuildApplication(new AgentAddress((Address)options.MemberAddress))
        {
        };
        await _blockChain.SendTransactionAsync([action], cancellationToken);
    }

    public async Task RejectJoinAsync(
        RejectJoinOptions options, CancellationToken cancellationToken)
    {
        ThrowIfNotRunning();

        var action = new RejectGuildApplication(new AgentAddress((Address)options.MemberAddress))
        {
        };
        await _blockChain.SendTransactionAsync([action], cancellationToken);
    }

    public async Task QuitAsync(LeaveGuildOptions options, CancellationToken cancellationToken)
    {
        ThrowIfNotRunning();

        var quitGuild = new QuitGuild
        {
        };
        await _blockChain.SendTransactionAsync([quitGuild], cancellationToken);
    }

    public async Task BanMemberAsync(BanMemberOptions options, CancellationToken cancellationToken)
    {
        var memberAddress = (Address)options.MemberAddress;
        var banGuildMember = new BanGuildMember(new(memberAddress))
        {
        };
        await _blockChain.SendTransactionAsync([banGuildMember], cancellationToken);
    }

    public async Task UnbanMemberAsync(
        UnbanMemberOptions options, CancellationToken cancellationToken)
    {
        ThrowIfNotRunning();

        var memberAddress = (Address)options.MemberAddress;
        var unbanMemberGuild = new UnbanGuildMember(memberAddress)
        {
        };
        await _blockChain.SendTransactionAsync([unbanMemberGuild], cancellationToken);
    }

    public Task<Address> GetGuildAsync(
        long height, Address address, CancellationToken cancellationToken)
        => throw new NotImplementedException();

    public Task<Address[]> GetGuildMembersAsync(
        long height, Address guildAddress, CancellationToken cancellationToken)
        => throw new NotImplementedException();

    public void Dispose()
    {
        _client.Started -= Client_Started;
        _client.Stopped -= Client_Stopped;
    }

    private async void Client_Started(object? sender, EventArgs e)
    {
        // Info = await _remoteGuildService.Service.GetGuildInfoAsync(default);

        _isRunning = true;
    }

    private void Client_Stopped(object? sender, EventArgs e)
    {
        Info = default;
        _isRunning = false;
    }

    private void ThrowIfNotRunning()
    {
        if (_client.IsRunning != true || _isRunning != true)
        {
            throw new InvalidOperationException("The guild is not running.");
        }
    }
}

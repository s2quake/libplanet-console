using System.ComponentModel.Composition;
using Libplanet.Crypto;
using LibplanetConsole.Frameworks;
using LibplanetConsole.Guild;
using Nekoyume.Action.Guild;
using Nekoyume.TypedAddress;

namespace LibplanetConsole.Clients.Guild;

[Export(typeof(IGuildClient))]
[Export(typeof(IApplicationService))]
[Export]
internal sealed class GuildClient : IGuildClient, IApplicationService, IDisposable
{
    private readonly IClient _client;
    private readonly IBlockChain _blockChain;
    private bool _isEnabled;

    [ImportingConstructor]
    public GuildClient(IClient client, IBlockChain blockChain)
    {
        _client = client;
        _blockChain = blockChain;
        _client.Started += Client_Started;
        _client.Stopped += Client_Stopped;
    }

    public GuildInfo Info { get; private set; }

    public bool IsRunning { get; private set; }

    public async Task<GuildInfo> StartAsync(CancellationToken cancellationToken)
    {
        if (IsRunning == true)
        {
            throw new InvalidOperationException("The guild is already running.");
        }

        await Task.Delay(1, cancellationToken);
        Info = new() { IsRunning = true, };
        IsRunning = true;
        return Info;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (IsRunning == false)
        {
            throw new InvalidOperationException("The guild is not running.");
        }

        await Task.Delay(1, cancellationToken);
        Info = default;
        IsRunning = false;
    }

    public async Task CreateAsync(CreateGuildOptions options, CancellationToken cancellationToken)
    {
        ThrowIfNotRunning();

        var makeGuild = new MakeGuild
        {
        };
        await _blockChain.SendTransactionAsync([makeGuild], cancellationToken);
    }

    public async Task DeleteAsync(DeleteGuildOptions options, CancellationToken cancellationToken)
    {
        ThrowIfNotRunning();

        var removeGuild = new RemoveGuild
        {
        };
        await _blockChain.SendTransactionAsync([removeGuild], cancellationToken);
    }

    public async Task RequestJoinAsync(RequestJoinOptions options, CancellationToken cancellationToken)
    {
        ThrowIfNotRunning();

        var action = new ApplyGuild(new GuildAddress((Address)options.GuildAddress))
        {
        };
        await _blockChain.SendTransactionAsync([action], cancellationToken);
    }

    public async Task CancelJoinAsync(CancelJoinOptions options, CancellationToken cancellationToken)
    {
        ThrowIfNotRunning();

        var action = new CancelGuildApplication()
        {
        };
        await _blockChain.SendTransactionAsync([action], cancellationToken);
    }

    public async Task AcceptJoinAsync(AcceptJoinOptions options, CancellationToken cancellationToken)
    {
        ThrowIfNotRunning();

        var action = new AcceptGuildApplication(new AgentAddress((Address)options.MemberAddress))
        {
        };
        await _blockChain.SendTransactionAsync([action], cancellationToken);
    }

    public async Task RejectJoinAsync(RejectJoinOptions options, CancellationToken cancellationToken)
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

    public void Dispose()
    {
        _client.Started -= Client_Started;
        _client.Stopped -= Client_Stopped;
    }

    async Task IApplicationService.InitializeAsync(
        IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var settings = ApplicationSettingsParser.Peek<GuildClientSettings>();
        _isEnabled = settings.NoGuildClient != true;
        await Task.CompletedTask;
    }

    private async void Client_Started(object? sender, EventArgs e)
    {
        if (_isEnabled == true && IsRunning != true)
        {
            await StartAsync(default);
        }
    }

    private async void Client_Stopped(object? sender, EventArgs e)
    {
        if (_isEnabled == true && IsRunning == true)
        {
            await StopAsync(default);
        }
    }

    private void ThrowIfNotRunning()
    {
        if (IsRunning != true)
        {
            throw new InvalidOperationException("The guild is not running.");
        }
    }
}

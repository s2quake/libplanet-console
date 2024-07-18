using System.ComponentModel.Composition;
using Bencodex;
using Libplanet.Blockchain;
using Libplanet.Crypto;
using LibplanetConsole.Common;
using LibplanetConsole.Frameworks;
using LibplanetConsole.Guild;
using Nekoyume.Action.Guild;
using Nekoyume.TypedAddress;

namespace LibplanetConsole.Nodes.Guild;

[Export(typeof(IGuildNode))]
[Export(typeof(IApplicationService))]
[Export]
internal sealed class GuildNode : IGuildNode, IApplicationService, IDisposable
{
    private static readonly Codec _codec = new();
    private readonly INode _node;
    private readonly IBlockChain _blockChain;
    private bool _isEnabled;

    [ImportingConstructor]
    public GuildNode(INode node, IBlockChain blockChain)
    {
        _node = node;
        _blockChain = blockChain;
        _node.Started += Node_Started;
        _node.Stopped += Node_Stopped;
    }

    public GuildInfo Info { get; private set; }

    public bool IsRunning { get; private set; }

    public async Task<GuildInfo> StartAsync(CancellationToken cancellationToken)
    {
        if (IsRunning == true)
        {
            throw new InvalidOperationException("The guild is already running.");
        }

        Info = await GetGuildInfoAsync(cancellationToken) with { IsRunning = true };
        IsRunning = true;
        return Info;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (IsRunning == false)
        {
            throw new InvalidOperationException("The guild is not running.");
        }

        await Task.CompletedTask;
        Info = default;
        IsRunning = false;
    }

    public async Task CreateAsync(CreateGuildOptions options, CancellationToken cancellationToken)
    {
        ThrowIfNotRunning();

        var makeGuild = new MakeGuild
        {
        };
        await _blockChain.AddTransactionAsync([makeGuild], cancellationToken);
        Info = await GetGuildInfoAsync(cancellationToken) with { IsRunning = true };
    }

    public async Task DeleteAsync(DeleteGuildOptions options, CancellationToken cancellationToken)
    {
        ThrowIfNotRunning();

        var removeGuild = new RemoveGuild
        {
        };
        await _blockChain.AddTransactionAsync([removeGuild], cancellationToken);
        Info = await GetGuildInfoAsync(cancellationToken) with { IsRunning = true };
    }

    public async Task RequestJoinAsync(RequestJoinOptions options, CancellationToken cancellationToken)
    {
        ThrowIfNotRunning();

        var action = new ApplyGuild(new GuildAddress((Address)options.GuildAddress))
        {
        };
        await _blockChain.AddTransactionAsync([action], cancellationToken);
    }

    public async Task CancelJoinAsync(CancelJoinOptions options, CancellationToken cancellationToken)
    {
        ThrowIfNotRunning();

        var action = new CancelGuildApplication
        {
        };
        await _blockChain.AddTransactionAsync([action], cancellationToken);
    }

    public async Task AcceptJoinAsync(AcceptJoinOptions options, CancellationToken cancellationToken)
    {
        ThrowIfNotRunning();

        var action = new AcceptGuildApplication(new AgentAddress((Address)options.MemberAddress))
        {
        };
        await _blockChain.AddTransactionAsync([action], cancellationToken);
    }

    public async Task RejectJoinAsync(RejectJoinOptions options, CancellationToken cancellationToken)
    {
        ThrowIfNotRunning();

        var action = new RejectGuildApplication(new AgentAddress((Address)options.MemberAddress))
        {
        };
        await _blockChain.AddTransactionAsync([action], cancellationToken);
    }

    public async Task JoinAsync(JoinGuildOptions options, CancellationToken cancellationToken)
    {
        ThrowIfNotRunning();

        // var guildAddress = (Address)options.GuildAddress;
        // var joinGuild = new JoinGuild(new GuildAddress(guildAddress))
        // {
        // };
        // await _blockChain.AddTransactionAsync([joinGuild], cancellationToken);
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

    public void Dispose()
    {
        _node.Started -= Node_Started;
        _node.Stopped -= Node_Stopped;
    }

    async Task IApplicationService.InitializeAsync(
        IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var settings = ApplicationSettingsParser.Peek<GuildNodeSettings>();
        _isEnabled = settings.NoGuildNode != true;
        await Task.CompletedTask;
    }

    private async Task<GuildInfo> GetGuildInfoAsync(CancellationToken cancellationToken)
    {
        try
        {
            var tipHash = await _blockChain.GetTipHashAsync(cancellationToken);
            var guildAddress = _node.Address;
            var guildData = await _blockChain.GetStateByBlockHashAsync(
                tipHash, (AppAddress)Nekoyume.Addresses.Guild, guildAddress, cancellationToken);
            if (guildData.Length > 0)
            {
                var guildValue = _codec.Decode(guildData);
                return new GuildInfo(guildValue);
            }
        }
        catch
        {
            // ignored
        }

        return default;
    }

    private async void Node_Started(object? sender, EventArgs e)
    {
        if (_isEnabled == true && IsRunning != true)
        {
            await StartAsync(default);
        }
    }

    private async void Node_Stopped(object? sender, EventArgs e)
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

using System.Net;
using Bencodex.Types;
using JSSoft.Communication;
using Libplanet.Blockchain;
using Libplanet.Crypto;
using LibplanetConsole.ClientServices;
using LibplanetConsole.ClientServices.Games.Serializations;
using LibplanetConsole.ClientServices.Serializations;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Exceptions;

namespace LibplanetConsole.Executable;

internal sealed class Client :
    IClientCallback, IAsyncDisposable, IIdentifier, IClient
{
    private readonly PrivateKey _privateKey;
    private readonly ClientContext _clientContext;
    private readonly ClientService<IClientService, IClientCallback> _clientService;
    private Guid _closeToken;
    private ClientInfo _clientInfo = new();

    public Client(PrivateKey privateKey, EndPoint endPoint)
    {
        _privateKey = privateKey;
        _clientService = new(this);
        _clientContext = new ClientContext(
            _clientService)
        {
            EndPoint = endPoint,
        };
        _clientContext.Disconnected += ClientContext_Disconnected;
        _clientContext.Faulted += ClientContext_Faulted;
    }

    public event EventHandler? Disposed;

    public PrivateKey PrivateKey => _privateKey;

    public PublicKey PublicKey => _privateKey.PublicKey;

    public Address Address => _privateKey.Address;

    public PlayerInfo? PlayerInfo { get; set; }

    public bool IsOnline { get; private set; } = true;

    public TextWriter Out { get; set; } = Console.Out;

    public bool IsRunning { get; private set; }

    public string Identifier => Address.ToString()[0..8];

    public ClientOptions ClientOptions { get; private set; } = ClientOptions.Default;

    public EndPoint EndPoint => _clientContext.EndPoint;

    PrivateKey IIdentifier.PrivateKey => _privateKey;

    public static PlayerInfo? GetPlayerInfo(BlockChain blockChain, Address address)
    {
        var worldState = blockChain.GetWorldState();
        var account = worldState.GetAccountState(address);
        if (account.GetState(UserStates.PlayerInfo) is Dictionary values)
        {
            return new PlayerInfo(values);
        }

        return null;
    }

    public override string ToString()
    {
        return $"{Address.ToString()[0..8]}: {EndPointUtility.ToString(EndPoint)}";
    }

    public async Task<ClientInfo> GetInfoAsync(CancellationToken cancellationToken)
    {
        _clientInfo = await _clientService.Server.GetInfoAsync(cancellationToken);
        return _clientInfo;
    }

    public async Task StartAsync(ClientOptions clientOptions, CancellationToken cancellationToken)
    {
        _closeToken = await _clientContext.OpenAsync(cancellationToken);
        _clientInfo = await _clientService.Server.StartAsync(clientOptions, cancellationToken);
        ClientOptions = clientOptions;
        IsRunning = true;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        ClientOptions = ClientOptions.Default;
        IsRunning = false;
        await _clientService.Server.StopAsync(cancellationToken);
        await _clientContext.CloseAsync(_closeToken, cancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public void Login(BlockChain blockChain)
    {
        InvalidOperationExceptionUtility.ThrowIf(IsOnline == true, $"{this} is already online.");

        PlayerInfo = GetPlayerInfo(blockChain, Address);
        IsOnline = true;
        Out.WriteLine($"{this} is logged in.");
    }

    public void Logout()
    {
        InvalidOperationExceptionUtility.ThrowIf(IsOnline != true, $"{this} is not online.");

        PlayerInfo = null;
        IsOnline = false;
        Out.WriteLine($"{this} is logged out.");
    }

    public PlayerInfo GetPlayerInfo()
    {
        InvalidOperationExceptionUtility.ThrowIf(IsOnline != true, $"{this} is not online.");
        InvalidOperationExceptionUtility.ThrowIf(
            condition: PlayerInfo == null,
            message: $"{this} does not have character.");

        return PlayerInfo!;
    }

    public GamePlayRecord[] GetGameHistory(BlockChain blockChain)
    {
        InvalidOperationExceptionUtility.ThrowIf(IsOnline != true, $"{this} is not online.");

        return GamePlayRecord.GetGamePlayRecords(blockChain, Address).ToArray();
    }

    public void Refresh(BlockChain blockChain)
    {
        InvalidOperationExceptionUtility.ThrowIf(IsOnline != true, $"{this} is not online.");

        PlayerInfo = GetPlayerInfo(blockChain, Address);
    }

    public Task<long> PlayGameAsync(BlockChain blockChain, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }

    public Task ReplayGameAsync(
        BlockChain blockChain, int tick, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }

    public Task ReplayGameAsync(
        BlockChain blockChain, long blockIndex, int tick, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }

    public Task CreateCharacterAsync(Client client, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }

    public Task ReviveCharacterAsync(Client client, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }

    public object? GetService(Type serviceType)
    {
        return null;
    }

    private void ClientContext_Disconnected(object? sender, EventArgs e)
    {
        Disposed?.Invoke(this, EventArgs.Empty);
    }

    private void ClientContext_Faulted(object? sender, EventArgs e)
    {
        Disposed?.Invoke(this, EventArgs.Empty);
    }
}

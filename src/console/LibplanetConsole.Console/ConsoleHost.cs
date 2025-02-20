using LibplanetConsole.BlockChain.Converters;
using LibplanetConsole.Console.Extensions;

namespace LibplanetConsole.Console;

internal sealed partial class ConsoleHost(
    IServiceProvider serviceProvider,
    NodeCollection nodes,
    ClientCollection clients,
    IApplicationOptions options,
    ILogger<ConsoleHost> logger) : IConsole
{
    private readonly PrivateKey _privateKey = options.PrivateKey;
    private readonly BlockHash _genesisHash = options.GenesisBlock.Hash;
    private IConsoleContent[]? _contents;
    private CancellationTokenSource? _cancellationTokenSource;

    static ConsoleHost()
    {
        AddressTypeConverter.Register();
    }

    public event EventHandler? Started;

    public event EventHandler? Stopped;

    public bool IsRunning { get; private set; }

    public Address Address => _privateKey.Address;

    public IConsoleContent[] Contents
    {
        get => _contents ?? throw new InvalidOperationException("Contents is not initialized.");
        set => _contents = value;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (IsRunning is true)
        {
            throw new InvalidOperationException("Node is already running.");
        }

        _cancellationTokenSource = new();
        IsRunning = true;
        logger.LogDebug("Console is started: {Address}", Address);
        await Contents.StartAsync(cancellationToken);
        logger.LogDebug("Console Contents are started: {Address}", Address);
        Started?.Invoke(this, EventArgs.Empty);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (IsRunning is false)
        {
            throw new InvalidOperationException("Node is not running.");
        }

        await Contents.StopAsync(cancellationToken);
        logger.LogDebug("Console Contents are stopped: {Address}", Address);

        if (_cancellationTokenSource is not null)
        {
            await _cancellationTokenSource.CancelAsync();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }

        IsRunning = false;
        logger.LogDebug("Console is stopped: {Address}", Address);
        Stopped?.Invoke(this, EventArgs.Empty);
    }

    public async Task InitializeAsync()
    {
        _cancellationTokenSource = new();
        try
        {
            await nodes.InitializeAsync(_cancellationTokenSource.Token);
            await clients.InitializeAsync(_cancellationTokenSource.Token);
        }
        catch (OperationCanceledException e)
        {
            logger.LogDebug(e, "The console was canceled.");
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while starting the console.");
        }
    }

    public async Task<TxId> SendTransactionAsync(
        IAction[] actions, CancellationToken cancellationToken)
    {
        if (IsRunning is false)
        {
            throw new InvalidOperationException("BlockChain is not running.");
        }

        var blockChain = serviceProvider.GetRequiredService<ConsoleBlockChain>();
        var privateKey = _privateKey;
        var genesisHash = _genesisHash;
        var nonce = await blockChain.GetNextNonceAsync(privateKey.Address, cancellationToken);
        var values = actions.Select(item => item.PlainValue).ToArray();
        var transaction = Transaction.Create(
            nonce: nonce,
            privateKey: privateKey,
            genesisHash: genesisHash,
            actions: [.. values]);

        await blockChain.SendTransactionAsync(transaction, cancellationToken);
        return transaction.Id;
    }
}

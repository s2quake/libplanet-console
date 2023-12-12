using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using JSSoft.Library.Commands;
using JSSoft.Library.Terminals;
using Libplanet.Blockchain;
using Libplanet.Types.Blocks;

namespace OnBoarding.ConsoleHost;

sealed partial class Application : IAsyncDisposable
{
    private readonly CompositionContainer _container;
    private readonly SwarmHostCollection _swarmHosts;
    private readonly UserCollection _users;
    private CancellationTokenSource? _cancellationTokenSource;
    private bool _isDisposed;
    private SystemTerminal? _terminal;
    private readonly ApplicationOptions _options = new();

    public Application(ApplicationOptions options)
    {
        Thread.CurrentThread.Priority = ThreadPriority.Highest;
        SynchronizationContext.SetSynchronizationContext(new());
        _options = options;
        _container = new(new AssemblyCatalog(typeof(Application).Assembly));
        _container.ComposeExportedValue(this);
        _container.ComposeExportedValue(_options);
        _swarmHosts = _container.GetExportedValue<SwarmHostCollection>()!;
        _users = _container.GetExportedValue<UserCollection>()!;
    }

    public User GetUser(int userIndex)
    {
        if (_users == null)
            throw new InvalidOperationException();
        return userIndex == -1 ? _users.CurrentUser : _users[userIndex];
    }

    public BlockChain GetBlockChain(int swarmIndex)
    {
        if (_swarmHosts == null)
            throw new InvalidOperationException();
        return swarmIndex == -1 ? _swarmHosts.CurrentSwarmHost.BlockChain : _swarmHosts[swarmIndex].BlockChain;
    }

    public Block GetBlock(int swarmIndex, int blockIndex)
    {
        var blockChain = GetBlockChain(swarmIndex);
        return blockIndex == -1 ? blockChain[blockChain.Count - 1] : blockChain[blockIndex];
    }

    public SwarmHost GetSwarmHost(int swarmIndex)
    {
        if (_swarmHosts == null)
            throw new InvalidOperationException();
        return swarmIndex == -1 ? _swarmHosts.CurrentSwarmHost : _swarmHosts[swarmIndex];
    }

    public void Cancel()
    {
        if (_isDisposed == true)
            throw new ObjectDisposedException($"{this}");

        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = null;
    }

    public async Task StartAsync()
    {
        if (_isDisposed == true)
            throw new ObjectDisposedException($"{this}");
        if (_terminal != null)
            throw new InvalidOperationException("Application has already been started.");

        await _swarmHosts.InitializeAsync(cancellationToken: default);
        _swarmHosts.CurrentSwarmHost.BlockAppended += SwarmHost_BlockAppended;
        await PrepareCommandContext();
        _cancellationTokenSource = new();
        _terminal = _container.GetExportedValue<SystemTerminal>()!;
        await _terminal!.StartAsync(_cancellationTokenSource.Token);

        async Task PrepareCommandContext()
        {
            var sw = new StringWriter();
            var commandContext = GetService<CommandContext>()!;
            commandContext.Out = sw;
            sw.WriteLine(TerminalStringBuilder.GetString("============================================================", TerminalColorType.BrightGreen));
            await commandContext.ExecuteAsync(new string[] { "--help" }, cancellationToken: default, progress: new Progress<ProgressInfo>());
            sw.WriteLine();
            await commandContext.ExecuteAsync(Array.Empty<string>(), cancellationToken: default, progress: new Progress<ProgressInfo>());
            sw.WriteLine(TerminalStringBuilder.GetString("============================================================", TerminalColorType.BrightGreen));
            commandContext.Out = Console.Out;
            Console.Write(sw.ToString());
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_isDisposed == true)
            throw new ObjectDisposedException($"{this}");

        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = null;
        await _swarmHosts.DisposeAsync();
        _terminal = null;
        _container.Dispose();
        _isDisposed = true;
        GC.SuppressFinalize(this);
    }

    public T? GetService<T>()
    {
        return _container.GetExportedValue<T>();
    }

    public object? GetService(Type serviceType)
    {
        return _container.GetExportedValue<object?>(AttributedModelServices.GetContractName(serviceType));
    }

    private void SwarmHost_BlockAppended(object? sender, EventArgs e)
    {
        if (sender is SwarmHost swarmHost)
        {
            var blockChain = swarmHost.BlockChain;
            Console.WriteLine($"Block Appended: {blockChain.Count}");
        }
    }
}

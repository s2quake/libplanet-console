using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using JSSoft.Library.Commands.Extensions;
using JSSoft.Library.Terminals;
using Libplanet.Blockchain;
using Libplanet.Types.Blocks;

namespace OnBoarding.ConsoleHost;

sealed partial class Application : IAsyncDisposable, IServiceProvider
{
    private readonly CompositionContainer _container;
    private readonly SwarmHostCollection _swarmHosts;
    private readonly UserCollection _users;
    private readonly ApplicationServiceCollection _applicationServices;
    private CancellationTokenSource? _cancellationTokenSource;
    private bool _isDisposed;
    private SystemTerminal? _terminal;
    private readonly ApplicationOptions _options = new();
    private SwarmHost _currentSwarmHost;

    public Application(ApplicationOptions options)
    {
        Thread.CurrentThread.Priority = ThreadPriority.Highest;
        SynchronizationContext.SetSynchronizationContext(new());
        _options = options;
        _container = new(new AssemblyCatalog(typeof(Application).Assembly));
        _container.ComposeExportedValue(this);
        _container.ComposeExportedValue<IServiceProvider>(this);
        _container.ComposeExportedValue(_options);
        _swarmHosts = _container.GetExportedValue<SwarmHostCollection>()!;
        _users = _container.GetExportedValue<UserCollection>()!;
        _applicationServices = new(_container.GetExportedValues<IApplicationService>());
        _currentSwarmHost = _swarmHosts.Current;
        _currentSwarmHost.BlockAppended += SwarmHost_BlockAppended;
        _swarmHosts.CurrentChanged += SwarmHosts_CurrentChanged;
    }

    public User GetUser(int userIndex)
    {
        if (_users == null)
            throw new InvalidOperationException();
        return userIndex == -1 ? _users.Current : _users[userIndex];
    }

    public BlockChain GetBlockChain(int swarmIndex)
    {
        if (_swarmHosts == null)
            throw new InvalidOperationException();
        return swarmIndex == -1 ? _swarmHosts.Current.BlockChain : _swarmHosts[swarmIndex].BlockChain;
    }

    public Block GetBlock(int swarmIndex, long blockIndex)
    {
        var blockChain = GetBlockChain(swarmIndex);
        return blockIndex == -1 ? blockChain[blockChain.Count - 1] : blockChain[blockIndex];
    }

    public SwarmHost GetSwarmHost(int swarmIndex)
    {
        if (_swarmHosts == null)
            throw new InvalidOperationException();
        return swarmIndex == -1 ? _swarmHosts.Current : _swarmHosts[swarmIndex];
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

        await _applicationServices.InitializeAsync(this, cancellationToken: default);
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
            await commandContext.ExecuteAsync(new string[] { "--help" }, cancellationToken: default);
            sw.WriteLine();
            await commandContext.ExecuteAsync(Array.Empty<string>(), cancellationToken: default);
            sw.WriteLine(TerminalStringBuilder.GetString("============================================================", TerminalColorType.BrightGreen));
            commandContext.Out = Console.Out;
            Console.Write(sw.ToString());
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_isDisposed == true)
            throw new ObjectDisposedException($"{this}");

        _swarmHosts.CurrentChanged -= SwarmHosts_CurrentChanged;
        _currentSwarmHost.BlockAppended -= SwarmHost_BlockAppended;
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = null;
        _container.Dispose();
        await _applicationServices.DisposeAsync();
        _terminal = null;
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
            Console.WriteLine(TerminalStringBuilder.GetString($"Block Appended: {blockChain.Count}", TerminalColorType.BrightCyan));
        }
    }

    private void SwarmHosts_CurrentChanged(object? sender, EventArgs e)
    {
        _currentSwarmHost.BlockAppended -= SwarmHost_BlockAppended;
        _currentSwarmHost = _swarmHosts.Current;
        _currentSwarmHost.BlockAppended += SwarmHost_BlockAppended;
        Console.WriteLine($"Current Swarm: {_currentSwarmHost}");
    }
}

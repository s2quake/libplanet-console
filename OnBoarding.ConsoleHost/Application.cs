using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Bencodex.Types;
using JSSoft.Library.Commands;
using JSSoft.Library.Terminals;
using Libplanet.Blockchain;
using OnBoarding.ConsoleHost.Games.Serializations;

namespace OnBoarding.ConsoleHost;

sealed partial class Application : IAsyncDisposable, IServiceProvider
{
    private readonly CompositionContainer _container;
    private SwarmHostCollection? _swarmHosts;
    private UserCollection? _users;
    private CancellationTokenSource? _cancellationTokenSource;
    private bool _isDisposed;
    private SystemTerminal? _terminal;

    static Application()
    {
        // Log.Logger = new LoggerConfiguration().MinimumLevel.Error()
        //                                       .WriteTo.Console()
        //                                       .CreateLogger();
    }

    public Application()
    {
        _container = new(new AssemblyCatalog(typeof(Application).Assembly));
        _container.ComposeExportedValue(this);
    }

    public int CurrentIndex { get; } = 0;

    public User GetUser(int index)
    {
        if (_users == null)
            throw new InvalidOperationException();
        return index == -1 ? _users[CurrentIndex] : _users[index];
    }

    public BlockChain GetBlockChain(int index)
    {
        if (_swarmHosts == null)
            throw new InvalidOperationException();
        return index == -1 ? _swarmHosts[CurrentIndex].BlockChain : _swarmHosts[index].BlockChain;
    }

    public PlayerInfo GetPlayerInfo(int swarmIndex) => GetPlayerInfo(swarmIndex, blockIndex: -1);

    public PlayerInfo GetPlayerInfo(int swarmIndex, int blockIndex)
    {
        var blockChain = GetBlockChain(swarmIndex);
        var user = GetUser(swarmIndex);
        var address = user.Address;
        var actualBlockIndex = blockIndex == -1 ? blockChain.Count - 1 : blockIndex;
        var block = blockChain[actualBlockIndex];
        var worldState = blockChain.GetWorldState(block.Hash);
        var account = worldState.GetAccount(address);
        if (account.GetState(address) is Dictionary values)
        {
            return new PlayerInfo(values);
        }
        return PlayerInfo.CreateNew(user.Name, address);
    }

    public void Cancel()
    {
        if (_isDisposed == true)
            throw new ObjectDisposedException($"{this}");

        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = null;
    }

    public async Task StartAsync(string[] args)
    {
        if (_isDisposed == true)
            throw new ObjectDisposedException($"{this}");
        if (_terminal != null)
            throw new InvalidOperationException("Application has already been started.");

        _users = _container.GetExportedValue<UserCollection>();
        _swarmHosts = _container.GetExportedValue<SwarmHostCollection>()!;
        await _swarmHosts.InitializeAsync(this, cancellationToken: default);
        await PrepareCommandContext(args);
        _cancellationTokenSource = new();
        _terminal = _container.GetExportedValue<SystemTerminal>()!;
        await _terminal!.StartAsync(_cancellationTokenSource.Token);

        async Task PrepareCommandContext(string[] args)
        {
            var @out = Console.Out;
            @out.WriteLine();
            if (GetService<CommandContext>() is { } commandContext)
            {
                @out.WriteLine(TerminalStringBuilder.GetString("============================================================", TerminalColorType.BrightGreen));
                await commandContext.ExecuteAsync(new string[] { "--help" }, cancellationToken: default, progress: new Progress<ProgressInfo>());
                @out.WriteLine(TerminalStringBuilder.GetString("============================================================", TerminalColorType.BrightGreen));
                @out.WriteLine();
                @out.WriteLine(TerminalStringBuilder.GetString("Type '--help | -h' for usage.", TerminalColorType.Red));
                @out.WriteLine(TerminalStringBuilder.GetString("Type 'exit' to exit application.", TerminalColorType.Red));
                @out.WriteLine();
                if (args.Length > 0)
                {
                    await commandContext.ExecuteAsync(args, cancellationToken: default, progress: new Progress<ProgressInfo>());
                }
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_isDisposed == true)
            throw new ObjectDisposedException($"{this}");

        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = null;
        if (_swarmHosts != null)
            await _swarmHosts.DisposeAsync();
        _swarmHosts = null;
        _users = null;
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
}

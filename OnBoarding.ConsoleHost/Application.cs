using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Bencodex.Types;
using JSSoft.Library.Commands;
using JSSoft.Library.Terminals;
using Libplanet.Blockchain;
using OnBoarding.ConsoleHost.Games;
using OnBoarding.ConsoleHost.Games.Serializations;
using Serilog;

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
        // Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
        //                                       .WriteTo.Console()
        //                                       .CreateLogger();
    }

    public Application()
    {
        _container = new(new AssemblyCatalog(typeof(Application).Assembly));
        _container.ComposeExportedValue(this);
    }

    public int CurrentIndex { get; } = 0;

    public BlockChain CurrentBlockChain
    {
        get
        {
            if (_terminal == null)
                throw new InvalidOperationException("Application has already been stopped.");

            return _swarmHosts![CurrentIndex].BlockChain;
        }
    }

    public User CurrentUser
    {
        get
        {
            if (_terminal == null)
                throw new InvalidOperationException("Application has already been stopped.");

            return _users![CurrentIndex];
        }
    }

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
        return new PlayerInfo
        {
            Name = $"Player #{user.Index}",
            Address = address,
            Life = 1000,
            MaxLife = 1000,
            Skills =
            [
                new SkillInfo{ MaxCoolTime = 3L, CoolTime = 0L, Value = new ValueRange(1, 4) },
            ],
        };
    }

    public void Cancel()
    {
        ObjectDisposedException.ThrowIf(condition: _isDisposed, this);

        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = null;
    }

    public async Task StartAsync(string[] args)
    {
        ObjectDisposedException.ThrowIf(condition: _isDisposed, this);
        if (_terminal != null)
            throw new InvalidOperationException("Application has already been started.");

        _users = _container.GetExportedValue<UserCollection>();
        _swarmHosts = _container.GetExportedValue<SwarmHostCollection>()!;
        await _swarmHosts.InitializeAsync(this, cancellationToken: default);
        Console.WriteLine();
        if (GetService<CommandContext>() is { } commandContext)
        {
            Console.WriteLine(TerminalStringBuilder.GetString("============================================================", TerminalColorType.BrightGreen));
            await commandContext.ExecuteAsync(["--help"], cancellationToken: default, progress: new Progress<ProgressInfo>());
            Console.WriteLine(TerminalStringBuilder.GetString("============================================================", TerminalColorType.BrightGreen));
            Console.WriteLine();
            Console.WriteLine(TerminalStringBuilder.GetString("Type '--help | -h' for usage.", TerminalColorType.Red));
            Console.WriteLine(TerminalStringBuilder.GetString("Type 'exit' to exit application.", TerminalColorType.Red));
            Console.WriteLine();
            if (args.Length > 0)
            {
                await commandContext.ExecuteAsync(args, cancellationToken: default, progress: new Progress<ProgressInfo>());
            }
        }
        _cancellationTokenSource = new();
        _terminal = _container.GetExportedValue<SystemTerminal>()!;
        await _terminal!.StartAsync(_cancellationTokenSource.Token);
    }

    public async ValueTask DisposeAsync()
    {
        ObjectDisposedException.ThrowIf(condition: _isDisposed, this);

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

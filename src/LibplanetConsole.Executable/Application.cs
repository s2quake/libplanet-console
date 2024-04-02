using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Text.RegularExpressions;
using JSSoft.Commands.Extensions;
using JSSoft.Terminals;
using Libplanet.Blockchain;
using Libplanet.Crypto;
using Libplanet.Types.Blocks;
using LibplanetConsole.Executable.Exceptions;

namespace LibplanetConsole.Executable;

sealed partial class Application : IAsyncDisposable, IServiceProvider
{
    private readonly CompositionContainer _container;
    private readonly NodeCollection _nodes;
    private readonly ClientCollection _clients;
    private readonly ApplicationServiceCollection _applicationServices;
    private CancellationTokenSource? _cancellationTokenSource;
    private bool _isDisposed;
    private SystemTerminal? _terminal;
    private readonly ApplicationOptions _options = new();
    private Node _currentNode;

    public Application(ApplicationOptions options)
    {
        Thread.CurrentThread.Priority = ThreadPriority.Highest;
        SynchronizationContext.SetSynchronizationContext(new());
        ConsoleTextWriter.SynchronizationContext = SynchronizationContext.Current!;
        _options = options;
        _container = new(new AssemblyCatalog(typeof(Application).Assembly));
        _container.ComposeExportedValue(this);
        _container.ComposeExportedValue<IServiceProvider>(this);
        _container.ComposeExportedValue(_options);
        _nodes = _container.GetExportedValue<NodeCollection>()!;
        _clients = _container.GetExportedValue<ClientCollection>()!;
        _applicationServices = new(_container.GetExportedValues<IApplicationService>());
        _currentNode = _nodes.Current;
        _currentNode.BlockAppended += Node_BlockAppended;
        _nodes.CurrentChanged += Nodes_CurrentChanged;
    }

    public Client GetClient(int clientIndex)
        => clientIndex == -1 ? _clients.Current : _clients[clientIndex];

    public Client GetClient(string identifier)
    {
        if (Regex.Match(identifier, @"c(\d+)") is { } match && match.Success == true)
        {
            var index = int.Parse(match.Groups[1].Value);
            return _clients[index];
        }
        return _clients[new Address(identifier)];
    }

    public BlockChain GetBlockChain(int nodeIndex)
        => nodeIndex == -1 ? _nodes.Current.BlockChain : _nodes[nodeIndex].BlockChain;

    public Block GetBlock(int nodeIndex, long blockIndex)
    {
        var blockChain = GetBlockChain(nodeIndex);
        return blockIndex == -1 ? blockChain[blockChain.Count - 1] : blockChain[blockIndex];
    }

    public Node GetNode(int nodeIndex)
        => nodeIndex == -1 ? _nodes.Current : _nodes[nodeIndex];

    public Node GetNode(string identifier)
    {
        if (Regex.Match(identifier, @"n(\d+)") is { } match && match.Success == true)
        {
            var index = int.Parse(match.Groups[1].Value);
            return _nodes[index];
        }
        return _nodes[new Address(identifier)];
    }

    public Address GetAddress(string identifier)
    {
        if (Regex.Match(identifier, @"([nc])(\d+)") is { } match && match.Success == true)
        {
            var index = int.Parse(match.Groups[2].Value);
            var type = match.Groups[1].Value;
            if (type == "n")
                return _nodes[index].Address;
            return _clients[index].Address;
        }

        var address = new Address(identifier);
        if (_nodes.Contains(address) == true || _clients.Contains(address) == true)
        {
            return address;
        }

        throw new ArgumentException("Invalid identifier.", nameof(identifier));
    }

    public string GetIdentifier(Address address)
    {
        if (_nodes.Contains(address) == true)
        {
            return $"n{_nodes.IndexOf(address)}";
        }

        if (_clients.Contains(address) == true)
        {
            return $"c{_clients.IndexOf(address)}";
        }

        throw new ArgumentException("Invalid address.", nameof(address));
    }

    public Address[] GetAddresses()
    {
        var addresses = new List<Address>();
        addresses.AddRange(_nodes.Select(item => item.Address));
        addresses.AddRange(_clients.Select(item => item.Address));
        return [.. addresses];
    }

    public void Cancel()
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);

        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = null;
    }

    public async Task StartAsync()
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);
        InvalidOperationExceptionUtility.ThrowIf(_terminal != null, "Application has already been started.");

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
            await commandContext.ExecuteAsync(["--help"], cancellationToken: default);
            sw.WriteLine();
            await commandContext.ExecuteAsync(Array.Empty<string>(), cancellationToken: default);
            sw.WriteLine(TerminalStringBuilder.GetString("============================================================", TerminalColorType.BrightGreen));
            commandContext.Out = Console.Out;
            Console.Write(sw.ToString());
        }
    }

    public async ValueTask DisposeAsync()
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);

        _nodes.CurrentChanged -= Nodes_CurrentChanged;
        _currentNode.BlockAppended -= Node_BlockAppended;
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

    private void Node_BlockAppended(object? sender, EventArgs e)
    {
        if (sender is Node node)
        {
            var blockChain = node.BlockChain;
            Console.WriteLine(TerminalStringBuilder.GetString($"Block Appended: #{blockChain.Tip.Index}", TerminalColorType.BrightCyan));
        }
    }

    private void Nodes_CurrentChanged(object? sender, EventArgs e)
    {
        _currentNode.BlockAppended -= Node_BlockAppended;
        _currentNode = _nodes.Current;
        _currentNode.BlockAppended += Node_BlockAppended;
        Console.WriteLine($"Current Swarm: {_currentNode}");
    }
}

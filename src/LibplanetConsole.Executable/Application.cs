using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using JSSoft.Commands.Extensions;
using JSSoft.Communication;
using JSSoft.Communication.Extensions;
using JSSoft.Terminals;
using Libplanet.Crypto;
using LibplanetConsole.Common;
using LibplanetConsole.Frameworks;
using LibplanetConsole.NodeServices;

namespace LibplanetConsole.Executable;

internal sealed partial class Application : ApplicationBase, IApplication
{
    private readonly CompositionContainer _container;
    private readonly NodeCollection _nodes;
    private readonly ClientCollection _clients;
    private readonly ApplicationOptions _options = new();
    private readonly ConsoleContext _serviceContext;
    private SystemTerminal? _terminal;
    private INode? _currentNode;
    private Guid _closeToken;

    public Application(ApplicationOptions options)
    {
        ConsoleTextWriter.SynchronizationContext = SynchronizationContext.Current!;
        _options = options;
        _container = new(new AssemblyCatalog(typeof(Application).Assembly));
        _container.ComposeExportedValue<IApplication>(this);
        _container.ComposeExportedValue<IServiceProvider>(this);
        _container.ComposeExportedValue(_options);
        _nodes = _container.GetExportedValue<NodeCollection>() ??
            throw new InvalidOperationException($"'{typeof(NodeCollection)}' is not found.");
        _clients = _container.GetExportedValue<ClientCollection>() ??
            throw new InvalidOperationException($"'{typeof(ClientCollection)}' is not found.");
        _serviceContext = _container.GetExportedValue<ConsoleContext>() ??
            throw new InvalidOperationException($"'{typeof(ConsoleContext)}' is not found.");
        ApplicationServices = new(_container.GetExportedValues<IApplicationService>());
        _nodes.CurrentChanged += Nodes_CurrentChanged;
    }

    public override ApplicationServiceCollection ApplicationServices { get; }

    public GenesisOptions GenesisOptions { get; } = GenesisOptions.Default;

    public IClient GetClient(string identifier)
    {
        if (identifier == string.Empty)
        {
            return _clients.Current ?? throw new InvalidOperationException("No node is selected.");
        }

        return _clients.Where(item => $"{item.Address}".StartsWith(identifier))
                     .Single();
    }

    public INode GetNode(string identifier)
    {
        if (identifier == string.Empty)
        {
            return _nodes.Current ?? throw new InvalidOperationException("No node is selected.");
        }

        return _nodes.Where(item => $"{item.Address}".StartsWith(identifier))
                     .Single();
    }

    public IIdentifier GetIdentifier(string identifier)
    {
        return _nodes.Concat<IIdentifier>(_clients)
                     .Where(item => $"{item.Address}".StartsWith(identifier))
                     .Single();
    }

    public IIdentifier GetIdentifier(Address address)
    {
        if (_nodes.Contains(address) == true)
        {
            return _nodes[address];
        }

        if (_clients.Contains(address) == true)
        {
            return _clients[address];
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

    public override object? GetService(Type serviceType)
    {
        var contractName = AttributedModelServices.GetContractName(serviceType);
        return _container.GetExportedValue<object?>(contractName);
    }

    protected override async Task OnStartAsync(CancellationToken cancellationToken)
    {
        _closeToken = await _serviceContext.OpenAsync(cancellationToken: default);
        await base.OnStartAsync(cancellationToken);
        await PrepareCommandContext();
        _terminal = _container.GetExportedValue<SystemTerminal>()!;
        await _terminal.StartAsync(cancellationToken);

        async Task PrepareCommandContext()
        {
            var separator = new string('=', 80);
            var sw = new StringWriter();
            var commandContext = _container.GetExportedValue<CommandContext>()!;
            commandContext.Out = sw;
            sw.WriteLine(TerminalStringBuilder.GetString(separator, TerminalColorType.BrightGreen));
            await commandContext.ExecuteAsync(["--help"], cancellationToken: default);
            sw.WriteLine();
            await commandContext.ExecuteAsync(args: [], cancellationToken: default);
            sw.WriteLine(TerminalStringBuilder.GetString(separator, TerminalColorType.BrightGreen));
            commandContext.Out = Console.Out;
            Console.Write(sw.ToString());
            Console.WriteLine(EndPointUtility.ToString(_serviceContext.EndPoint));
        }
    }

    protected override async ValueTask OnDisposeAsync()
    {
        await base.OnDisposeAsync();
        await _serviceContext.ReleaseAsync(_closeToken);
        _nodes.CurrentChanged -= Nodes_CurrentChanged;
        _container.Dispose();
    }

    private void Node_BlockAppended(object? sender, BlockEventArgs e)
    {
        var blockInfo = e.BlockInfo;
        var hash = blockInfo.Hash[0..8];
        var miner = blockInfo.Miner[0..8];
        var message = $"Block #{blockInfo.Index} '{hash}' Appended by '{miner}'";
        var coloredMessage = TerminalStringBuilder.GetString(message, TerminalColorType.BrightBlue);
        Console.WriteLine(coloredMessage);
    }

    private void Nodes_CurrentChanged(object? sender, EventArgs e)
    {
        if (_currentNode != null)
        {
            _currentNode.BlockAppended -= Node_BlockAppended;
        }

        _currentNode = _nodes.Current;

        if (_currentNode != null)
        {
            _currentNode.BlockAppended += Node_BlockAppended;
        }
    }
}

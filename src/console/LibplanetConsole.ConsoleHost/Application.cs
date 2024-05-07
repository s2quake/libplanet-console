using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using JSSoft.Commands.Extensions;
using JSSoft.Communication;
using JSSoft.Communication.Extensions;
using JSSoft.Terminals;
using Libplanet.Crypto;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Consoles;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.ConsoleHost;

internal sealed partial class Application : ApplicationBase, IApplication
{
    private readonly CompositionContainer _container;
    private readonly NodeCollection _nodes;
    private readonly ClientCollection _clients;
    private readonly ApplicationOptions _options = new();
    private readonly ConsoleContext _serviceContext;
    private readonly PrivateKey[] _reservedKeys;

    private SystemTerminal? _terminal;
    private Guid _closeToken;

    public Application(ApplicationOptions options)
    {
        _options = options;
        _container = new(
            new DirectoryCatalog(Path.GetDirectoryName(typeof(Application).Assembly.Location)!));
        _container.ComposeExportedValue(this);
        _container.ComposeExportedValue<IApplication>(this);
        _container.ComposeExportedValue<IServiceProvider>(this);
        _container.ComposeExportedValue(_options);
        _nodes = _container.GetExportedValue<NodeCollection>() ??
            throw new InvalidOperationException($"'{typeof(NodeCollection)}' is not found.");
        _clients = _container.GetExportedValue<ClientCollection>() ??
            throw new InvalidOperationException($"'{typeof(ClientCollection)}' is not found.");
        _serviceContext = _container.GetExportedValue<ConsoleContext>() ??
            throw new InvalidOperationException($"'{typeof(ConsoleContext)}' is not found.");
        _reservedKeys
            = [.. Enumerable.Range(0, options.NodeCount).Select(item => new PrivateKey())];
        GenesisOptions = new()
        {
            GenesisKey = new(),
            GenesisValidators = [.. _reservedKeys.Select(item => item.PublicKey)],
            Timestamp = DateTimeOffset.UtcNow,
        };
        ApplicationServices = new(_container.GetExportedValues<IApplicationService>());
    }

    public override ApplicationServiceCollection ApplicationServices { get; }

    public PrivateKey[] ReservedKeys => _reservedKeys;

    public GenesisOptions GenesisOptions { get; }

    public Client GetClient(string address)
    {
        if (address == string.Empty)
        {
            return _clients.Current ?? throw new InvalidOperationException("No node is selected.");
        }

        return _clients.Where<Client>(item => $"{item.Address}".StartsWith(address))
                       .Single();
    }

    public Node GetNode(string address)
    {
        if (address == string.Empty)
        {
            return _nodes.Current ?? throw new InvalidOperationException("No node is selected.");
        }

        return _nodes.Where<Node>(item => $"{item.Address}".StartsWith(address))
                     .Single();
    }

    public IAddressable GetAddressable(string address)
    {
        return _nodes.Concat<IAddressable>(_clients)
                     .Where(item => $"{item.Address}".StartsWith(address))
                     .Single();
    }

    public IAddressable GetAddressable(Address address)
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

    public override object? GetService(Type serviceType)
    {
        var contractName = AttributedModelServices.GetContractName(serviceType);
        return _container.GetExportedValue<object?>(contractName);
    }

    public CompositionContainer CreateChildContainer()
    {
        return new(
            catalog: new AssemblyCatalog(typeof(Application).Assembly),
            providers: _container);
    }

    IClient IApplication.GetClient(string address)
        => GetClient(address);

    INode IApplication.GetNode(string address)
        => GetNode(address);

    protected override async Task OnStartAsync(CancellationToken cancellationToken)
    {
        _terminal = _container.GetExportedValue<SystemTerminal>()!;
        _closeToken = await _serviceContext.OpenAsync(cancellationToken: default);
        await base.OnStartAsync(cancellationToken);
        await PrepareCommandContext();
        await _terminal.StartAsync(cancellationToken);

        async Task PrepareCommandContext()
        {
            var sw = new StringWriter();
            var commandContext = _container.GetExportedValue<CommandContext>()!;
            commandContext.Out = sw;
            sw.WriteSeparator(TerminalColorType.BrightGreen);
            await commandContext.ExecuteAsync(["--help"], cancellationToken: default);
            sw.WriteLine();
            await commandContext.ExecuteAsync(args: [], cancellationToken: default);
            sw.WriteSeparator(TerminalColorType.BrightGreen);
            commandContext.Out = Console.Out;
            Console.Write(sw.ToString());
            Console.WriteLine(EndPointUtility.ToString(_serviceContext.EndPoint));
        }
    }

    protected override async ValueTask OnDisposeAsync()
    {
        await base.OnDisposeAsync();
        await _serviceContext.ReleaseAsync(_closeToken);
        _container.Dispose();
    }
}

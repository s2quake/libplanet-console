using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using JSSoft.Communication.Extensions;
using Libplanet.Crypto;
using LibplanetConsole.Common;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.Consoles;

public abstract class ApplicationBase : Frameworks.ApplicationBase, IApplication
{
    private readonly CompositionContainer _container;
    private readonly NodeCollection _nodes;
    private readonly ClientCollection _clients;
    private readonly ApplicationOptions _options = new();
    private readonly ConsoleContext _consoleContext;
    private readonly PrivateKey[] _reservedKeys;

    private Guid _closeToken;

    protected ApplicationBase(ApplicationOptions options)
    {
        _options = options.GetActualOptions();
        _container = new(
            new DirectoryCatalog(Path.GetDirectoryName(GetType().Assembly.Location)!));
        _container.ComposeExportedValue(this);
        _container.ComposeExportedValue<IApplication>(this);
        _container.ComposeExportedValue<IServiceProvider>(this);
        _container.ComposeExportedValue(_options);
        _nodes = _container.GetExportedValue<NodeCollection>() ??
            throw new InvalidOperationException($"'{typeof(NodeCollection)}' is not found.");
        _clients = _container.GetExportedValue<ClientCollection>() ??
            throw new InvalidOperationException($"'{typeof(ClientCollection)}' is not found.");
        _consoleContext = _container.GetExportedValue<ConsoleContext>() ??
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

    public IClient GetClient(string address)
    {
        if (address == string.Empty)
        {
            return _clients.Current ?? throw new InvalidOperationException("No node is selected.");
        }

        return _clients.Where<Client>(item => $"{item.Address}".StartsWith(address))
                       .Single();
    }

    public INode GetNode(string address)
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
        var directoryName = Path.GetDirectoryName(GetType().Assembly.Location)!;
        return new(
            catalog: new DirectoryCatalog(directoryName),
            providers: new ApplicationExportProvider(_container));
    }

    protected override async Task OnStartAsync(CancellationToken cancellationToken)
    {
        await base.OnStartAsync(cancellationToken);
        _closeToken = await _consoleContext.OpenAsync(cancellationToken: default);
    }

    protected override async ValueTask OnDisposeAsync()
    {
        await base.OnDisposeAsync();
        await _consoleContext.ReleaseAsync(_closeToken);
        _container.Dispose();
    }
}

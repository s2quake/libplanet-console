using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Libplanet.Crypto;
using LibplanetConsole.Common;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.Consoles;

public abstract class ApplicationBase : Frameworks.ApplicationBase, IApplication
{
    private readonly CompositionContainer _container;
    private readonly NodeCollection _nodes;
    private readonly ClientCollection _clients;
    private readonly ConsoleServiceContext _consoleContext;

    private Guid _closeToken;

    protected ApplicationBase(ApplicationOptions options)
    {
        _container = new(
            new DirectoryCatalog(Path.GetDirectoryName(GetType().Assembly.Location)!));
        _nodes = new NodeCollection(this, options.Nodes, options.StoreDirectory);
        _clients = new ClientCollection(this, options.Clients);
        _container.ComposeExportedValue(this);
        _container.ComposeExportedValue<IApplication>(this);
        _container.ComposeExportedValue<IServiceProvider>(this);
        _container.ComposeExportedValue(_nodes);
        _container.ComposeExportedValue<INodeCollection>(_nodes);
        _container.ComposeExportedValue<IApplicationService>(_nodes);
        _container.ComposeExportedValue(_clients);
        _container.ComposeExportedValue<IClientCollection>(_clients);
        _container.ComposeExportedValue<IApplicationService>(_clients);
        _consoleContext = _container.GetExportedValue<ConsoleServiceContext>() ??
            throw new InvalidOperationException($"'{typeof(ConsoleServiceContext)}' is not found.");
        _consoleContext.EndPoint = options.EndPoint;
        GenesisOptions = new()
        {
            GenesisKey = new(),
            GenesisValidators = [.. options.Nodes.Select(item => item.PublicKey)],
            Timestamp = DateTimeOffset.UtcNow,
        };
        ApplicationServices = new(_container.GetExportedValues<IApplicationService>());
    }

    public override ApplicationServiceCollection ApplicationServices { get; }

    internal GenesisOptions GenesisOptions { get; }

    public IClient GetClient(Address? address)
    {
        if (address is null)
        {
            return _clients.Current ??
                throw new InvalidOperationException("No client is selected.");
        }

        return _clients.First<Client>(item => item.Address == address);
    }

    public INode GetNode(Address? address)
    {
        if (address is null)
        {
            return _nodes.Current ?? throw new InvalidOperationException("No node is selected.");
        }

        return _nodes.First<Node>(item => item.Address == address);
    }

    public IAddressable GetAddressable(Address address)
    {
        return _nodes.Concat<IAddressable>(_clients)
                     .First(item => item.Address == address);
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
        _closeToken = await _consoleContext.StartAsync(cancellationToken: default);
    }

    protected override async ValueTask OnDisposeAsync()
    {
        await base.OnDisposeAsync();
        await _consoleContext.CloseAsync(_closeToken, CancellationToken.None);
        _container.Dispose();
    }
}

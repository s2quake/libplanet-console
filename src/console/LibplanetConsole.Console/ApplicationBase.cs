using System.Diagnostics.CodeAnalysis;
using LibplanetConsole.Framework;
using LibplanetConsole.Node;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LibplanetConsole.Console;

public abstract class ApplicationBase : ApplicationFramework, IApplication
{
    private readonly IServiceProvider _serviceProvider;
    private readonly NodeCollection _nodes;
    private readonly ClientCollection _clients;
    private readonly ApplicationInfo _info;
    private readonly ILogger _logger;
    private ConsoleServiceContext? _consoleContext;
    private Guid _closeToken;

    protected ApplicationBase(IServiceProvider serviceProvider, ApplicationOptions options)
        : base(serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _logger = serviceProvider.GetRequiredService<ILogger<ApplicationBase>>();
        _logger.LogDebug(Environment.CommandLine);
        _logger.LogDebug("Application initializing...");
        _nodes = serviceProvider.GetRequiredService<NodeCollection>();
        _clients = serviceProvider.GetRequiredService<ClientCollection>();
        _info = new()
        {
            EndPoint = options.EndPoint,
            LogPath = options.LogPath,
            NoProcess = options.NoProcess,
            Detach = options.Detach,
            NewWindow = options.NewWindow,
        };
        GenesisBlock = BlockUtility.DeserializeBlock(options.Genesis);
        _logger.LogDebug("Application initialized.");
    }

    public ApplicationInfo Info => _info;

    internal Block GenesisBlock { get; }

    public bool TryGetClient(string address, [MaybeNullWhen(false)] out IClient client)
    {
        if (address == string.Empty)
        {
            client = _clients.Current;
            return client is not null;
        }

        client = _clients.SingleOrDefault<Client>(item => IsEquals(item, address));
        return client is not null;
    }

    public bool TryGetNode(string address, [MaybeNullWhen(false)] out INode node)
    {
        if (address == string.Empty)
        {
            node = _nodes.Current;
            return node is not null;
        }

        node = _nodes.SingleOrDefault<Node>(item => IsEquals(item, address));
        return node is not null;
    }

    public IAddressable GetAddressable(string address)
        => _nodes.Concat<IAddressable>(_clients).Single(item => IsEquals(item, address));

    public override object? GetService(Type serviceType)
        => _serviceProvider.GetService(serviceType);

    IClient IApplication.GetClient(string address) => GetClient(address);

    INode IApplication.GetNode(string address) => GetNode(address);

    internal Client GetClient(string address)
    {
        if (address == string.Empty)
        {
            return _clients.Current ?? throw new InvalidOperationException("No node is selected.");
        }

        return _clients.Single<Client>(item => IsEquals(item, address));
    }

    internal Node GetNode(string address)
    {
        if (address == string.Empty)
        {
            return _nodes.Current ?? throw new InvalidOperationException("No node is selected.");
        }

        return _nodes.Single<Node>(item => IsEquals(item, address));
    }

    protected override async Task OnRunAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("ConsoleContext is starting: {EndPoint}", _info.EndPoint);
        _consoleContext = _serviceProvider.GetRequiredService<ConsoleServiceContext>();
        _consoleContext.EndPoint = _info.EndPoint;
        _closeToken = await _consoleContext.StartAsync(cancellationToken: default);
        _logger.LogDebug("ConsoleContext is started: {EndPoint}", _info.EndPoint);
        await base.OnRunAsync(cancellationToken);
    }

    protected override async ValueTask OnDisposeAsync()
    {
        if (_consoleContext is not null)
        {
            _logger.LogDebug("ConsoleContext is closing: {EndPoint}", _info.EndPoint);
            await _consoleContext.CloseAsync(_closeToken, CancellationToken.None);
            _consoleContext = null;
            _logger.LogDebug("ConsoleContext is closed: {EndPoint}", _info.EndPoint);
        }

        await base.OnDisposeAsync();
    }

    private static bool IsEquals(IAddressable addressable, string address)
        => $"{addressable.Address}".StartsWith(address);
}

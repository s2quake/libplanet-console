using System.Collections;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using LibplanetConsole.Common;
using LibplanetConsole.Frameworks;
using LibplanetConsole.Frameworks.Extensions;
using Serilog;
using Serilog.Core;

namespace LibplanetConsole.Consoles;

public abstract class ApplicationBase : Frameworks.ApplicationBase, IApplication
{
    private readonly ApplicationContainer _container;
    private readonly NodeCollection _nodes;
    private readonly ClientCollection _clients;
    private readonly ConsoleServiceContext _consoleContext;
    private readonly ApplicationInfo _info;
    private readonly Logger _logger;
    private Guid _closeToken;

    protected ApplicationBase(ApplicationOptions options)
    {
        _logger = CreateLogger(options.LogDirectory);
        _logger.Debug(Environment.CommandLine);
        _logger.Debug("Application initializing...");
        _container = new(this);
        _container.ComposeExportedValue<ILogger>(_logger);
        _nodes = new NodeCollection(this, options.Nodes);
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
        _consoleContext = _container.GetValue<ConsoleServiceContext>();
        _consoleContext.EndPoint = options.EndPoint;
        _container.GetValue<IApplicationConfigurations>();
        _info = new()
        {
            EndPoint = _consoleContext.EndPoint,
            StoreDirectory = options.StoreDirectory,
            LogDirectory = options.LogDirectory,
            NoProcess = options.NoProcess,
            Detach = options.Detach,
            NewWindow = options.NewWindow,
            ManualStart = options.ManualStart,
        };
        GenesisOptions = new()
        {
            GenesisKey = GenesisOptions.AppProtocolKey,
            GenesisValidators = [.. options.Nodes.Select(item => item.PublicKey)],
            Timestamp = DateTimeOffset.UtcNow,
        };
        ApplicationServices = new(_container.GetExportedValues<IApplicationService>());
        _logger.Debug($"GenesisOptions: {JsonUtility.Serialize((GenesisInfo)GenesisOptions)}");
        _logger.Debug("Application initialized.");
    }

    public override ApplicationServiceCollection ApplicationServices { get; }

    public ApplicationInfo Info => _info;

    public override ILogger Logger => _logger;

    internal GenesisOptions GenesisOptions { get; }

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
    {
        return _nodes.Concat<IAddressable>(_clients).Single(item => IsEquals(item, address));
    }

    public override object? GetService(Type serviceType)
    {
        var isMultiple = serviceType.IsGenericType &&
            serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>);

        if (isMultiple == true)
        {
            var itemType = serviceType.GenericTypeArguments[0];
            var contractName = AttributedModelServices.GetContractName(itemType);
            var items = _container.GetExportedValues<object?>(contractName);
            var listGenericType = typeof(List<>);
            var list = listGenericType.MakeGenericType(itemType);
            var ci = list.GetConstructor([typeof(int)]) ?? throw new UnreachableException();
            var instance = (IList)ci.Invoke([items.Count(),]);
            foreach (var item in items)
            {
                instance.Add(item);
            }

            return instance;
        }
        else
        {
            var contractName = AttributedModelServices.GetContractName(serviceType);
            return _container.GetExportedValue<object?>(contractName);
        }
    }

    public ApplicationContainer CreateChildContainer(object owner)
        => new(owner, _container);

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
        _closeToken = await _consoleContext.StartAsync(cancellationToken: default);
        await base.OnRunAsync(cancellationToken);
    }

    protected override async ValueTask OnDisposeAsync()
    {
        await _consoleContext.CloseAsync(_closeToken, CancellationToken.None);
        await base.OnDisposeAsync();
        await _container.DisposeAsync();
    }

    private static Logger CreateLogger(string logDicrectory)
    {
        var loggerConfiguration = new LoggerConfiguration();
        if (logDicrectory != string.Empty)
        {
            var logFilename = Path.Combine(logDicrectory, "console.log");
            loggerConfiguration = loggerConfiguration.MinimumLevel.Debug()
                                                     .WriteTo.File(logFilename);
        }

        var logger = loggerConfiguration.CreateLogger();
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            logger.Fatal(e.ExceptionObject as Exception, "Unhandled exception occurred.");
        };

        return logger;
    }

    private static bool IsEquals(IAddressable addressable, string address)
        => $"{addressable.Address}".StartsWith(address);
}

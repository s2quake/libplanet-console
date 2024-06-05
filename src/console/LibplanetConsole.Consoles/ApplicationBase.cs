using System.Collections;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using LibplanetConsole.Common;
using LibplanetConsole.Consoles.Serializations;
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
        _logger.Information(Environment.CommandLine);
        _logger.Information("Initializing the application...");
        _container = new(this);
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
        _container.ComposeExportedValue<ILogger>(_logger);
        _consoleContext = _container.GetValue<ConsoleServiceContext>();
        _consoleContext.EndPoint = options.EndPoint;
        _container.GetValue<IApplicationConfigurations>();
        _info = new()
        {
            EndPoint = EndPointUtility.ToString(_consoleContext.EndPoint),
            StoreDirectory = options.StoreDirectory,
            LogDirectory = options.LogDirectory,
            ManualStart = options.ManualStart,
            IsNewTerminal = options.IsNewTerminal,
        };
        GenesisOptions = new()
        {
            GenesisKey = new(),
            GenesisValidators = [.. options.Nodes.Select(item => item.PublicKey)],
            Timestamp = DateTimeOffset.UtcNow,
        };
        ApplicationServices = new(_container.GetExportedValues<IApplicationService>());
        _logger.Information("Initialized the application.");
    }

    public override ApplicationServiceCollection ApplicationServices { get; }

    public ApplicationInfo Info => _info;

    public override ILogger Logger => _logger;

    internal GenesisOptions GenesisOptions { get; }

    public IClient GetClient(string address)
    {
        if (address == string.Empty)
        {
            return _clients.Current ?? throw new InvalidOperationException("No node is selected.");
        }

        return _clients.Where<Client>(item => IsEquals(item, address))
                       .Single();
    }

    public INode GetNode(string address)
    {
        if (address == string.Empty)
        {
            return _nodes.Current ?? throw new InvalidOperationException("No node is selected.");
        }

        return _nodes.Where<Node>(item => IsEquals(item, address))
                     .Single();
    }

    public bool TryGetClient(string address, [MaybeNullWhen(false)] out IClient client)
    {
        if (address == string.Empty)
        {
            client = _clients.Current;
            return client != null;
        }

        client = _clients.Where<Client>(item => IsEquals(item, address))
                         .SingleOrDefault();
        return client != null;
    }

    public bool TryGetNode(string address, [MaybeNullWhen(false)] out INode node)
    {
        if (address == string.Empty)
        {
            node = _nodes.Current;
            return node != null;
        }

        node = _nodes.Where<Node>(item => IsEquals(item, address))
                     .SingleOrDefault();
        return node != null;
    }

    public IAddressable GetAddressable(string address)
    {
        return _nodes.Concat<IAddressable>(_clients)
                     .Where(item => IsEquals(item, address))
                     .Single();
    }

    public override object? GetService(Type serviceType)
    {
        var isMultiple = serviceType.IsGenericType &&
            serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>);

        if (isMultiple == true)
        {
            var itemType = serviceType.GenericTypeArguments.First();
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
        => new(owner, _container, new ApplicationExportProvider(_container));

    protected override async Task OnStartAsync(CancellationToken cancellationToken)
    {
        _logger.Information("Starting the application...");
        await base.OnStartAsync(cancellationToken);
        _closeToken = await _consoleContext.StartAsync(cancellationToken: default);
        _logger.Information("Started the application.");
    }

    protected override async ValueTask OnDisposeAsync()
    {
        _logger.Information("Disposing the application...");
        await base.OnDisposeAsync();
        await _consoleContext.CloseAsync(_closeToken, CancellationToken.None);
        await _container.DisposeAsync();
        _logger.Information("Disposed the application.");
    }

    private static Logger CreateLogger(string logDicrectory)
    {
        var loggerConfiguration = new LoggerConfiguration();
        if (logDicrectory != string.Empty)
        {
            var logFilename = Path.Combine(logDicrectory, "console.log");
            loggerConfiguration = loggerConfiguration.WriteTo.File(logFilename);
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

using System.Collections;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using Libplanet.Crypto;
using LibplanetConsole.Common;
using LibplanetConsole.Consoles.Serializations;
using LibplanetConsole.Frameworks;
using LibplanetConsole.Frameworks.Extensions;
using Serilog;
using Serilog.Core;

namespace LibplanetConsole.Consoles;

public abstract class ApplicationBase : Frameworks.ApplicationBase, IApplication
{
    private readonly CompositionContainer _container;
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
        _container = new(
            new DirectoryCatalog(Path.GetDirectoryName(GetType().Assembly.Location)!));
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

    internal GenesisOptions GenesisOptions { get; }

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

    public CompositionContainer CreateChildContainer()
    {
        var directoryName = Path.GetDirectoryName(GetType().Assembly.Location)!;
        return new(
            catalog: new DirectoryCatalog(directoryName),
            providers: new ApplicationExportProvider(_container));
    }

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
        _container.Dispose();
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
}

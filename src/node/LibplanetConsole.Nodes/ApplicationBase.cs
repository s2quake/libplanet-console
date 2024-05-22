using System.Collections;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Net;
using LibplanetConsole.Common;
using LibplanetConsole.Frameworks;
using LibplanetConsole.Frameworks.Extensions;
using LibplanetConsole.Nodes.Serializations;
using LibplanetConsole.Nodes.Services;
using Serilog;
using Serilog.Core;

namespace LibplanetConsole.Nodes;

public abstract class ApplicationBase : Frameworks.ApplicationBase, IApplication
{
    private readonly CompositionContainer _container;
    private readonly Node _node;
    private readonly NodeContext _nodeContext;
    private readonly Process? _parentProcess;
    private readonly bool _isAutoStart;
    private readonly ApplicationInfo _info;
    private readonly Logger _logger;
    private Guid _closeToken;

    protected ApplicationBase(ApplicationOptions options)
    {
        _logger = CreateLogger(options.LogPath);
        _logger.Information(Environment.CommandLine);
        _logger.Information("Initializing the application...");
        _isAutoStart = options.AutoStart;
        _node = new Node(options);
        _container = new(
            new DirectoryCatalog(Path.GetDirectoryName(GetType().Assembly.Location)!));
        _container.ComposeExportedValue<IApplication>(this);
        _container.ComposeExportedValue(this);
        _container.ComposeExportedValue<IServiceProvider>(this);
        _container.ComposeExportedValue(_node);
        _container.ComposeExportedValue<INode>(_node);
        _container.ComposeExportedValue<ILogger>(_logger);
        _nodeContext = _container.GetValue<NodeContext>();
        _nodeContext.EndPoint = options.EndPoint;
        _container.GetValue<IApplicationConfigurations>();
        _info = new()
        {
            EndPoint = EndPointUtility.ToString(_nodeContext.EndPoint),
            NodeEndPoint = $"{options.NodeEndPoint}",
            StorePath = options.StorePath,
            LogPath = options.LogPath,
        };
        DefaultGenesisOptions = new GenesisOptions
        {
            GenesisKey = new(),
            GenesisValidators = options.GenesisValidators,
            Timestamp = DateTimeOffset.UtcNow,
        };
        ApplicationServices = new(_container.GetExportedValues<IApplicationService>());
        if (options.ParentProcessId != 0 &&
            Process.GetProcessById(options.ParentProcessId) is { } parentProcess)
        {
            _parentProcess = parentProcess;
        }

        _logger.Information("Initialized the application.");
    }

    public override ApplicationServiceCollection ApplicationServices { get; }

    public EndPoint EndPoint => _nodeContext.EndPoint;

    public ApplicationInfo Info => _info;

    internal GenesisOptions DefaultGenesisOptions { get; }

    protected override bool CanClose => _parentProcess?.HasExited == true;

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

    protected override async Task OnStartAsync(CancellationToken cancellationToken)
    {
        _logger.Information("Starting the application...");
        _closeToken = await _nodeContext.StartAsync(cancellationToken: default);
        await base.OnStartAsync(cancellationToken);
        await AutoStartAsync(cancellationToken);
        _logger.Information("Started the application.");
    }

    protected override async ValueTask OnDisposeAsync()
    {
        _logger.Information("Disposing the application...");
        await base.OnDisposeAsync();
        await _nodeContext.CloseAsync(_closeToken, cancellationToken: default);
        _container.Dispose();
        _logger.Information("Disposed the application.");
    }

    private static Logger CreateLogger(string logFilename)
    {
        var loggerConfiguration = new LoggerConfiguration();
        if (logFilename != string.Empty)
        {
            loggerConfiguration = loggerConfiguration.WriteTo.File(logFilename);
        }

        var logger = loggerConfiguration.CreateLogger();
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            logger.Fatal(e.ExceptionObject as Exception, "Unhandled exception occurred.");
        };

        return logger;
    }

    private async Task AutoStartAsync(CancellationToken cancellationToken)
    {
        if (_isAutoStart == true)
        {
            var seedEndPoint = _info.NodeEndPoint;
            var nodeOptions = seedEndPoint != string.Empty
                ? await NodeOptions.CreateAsync(seedEndPoint, cancellationToken)
                : new NodeOptions
                {
                    GenesisOptions = DefaultGenesisOptions,
                };
            await _node.StartAsync(nodeOptions, cancellationToken: default);
        }
    }
}

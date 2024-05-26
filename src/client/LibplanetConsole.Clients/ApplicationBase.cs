using System.Collections;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Net;
using LibplanetConsole.Clients.Serializations;
using LibplanetConsole.Clients.Services;
using LibplanetConsole.Common;
using LibplanetConsole.Frameworks;
using LibplanetConsole.Frameworks.Extensions;
using Serilog;
using Serilog.Core;

namespace LibplanetConsole.Clients;

public abstract class ApplicationBase : Frameworks.ApplicationBase, IApplication
{
    private readonly ApplicationContainer _container;
    private readonly Client _client;
    private readonly ClientServiceContext _clientServiceContext;
    private readonly Process? _parentProcess;
    private readonly bool _isSeed;
    private readonly ApplicationInfo _info;
    private readonly Logger _logger;
    private Guid _closeToken;

    protected ApplicationBase(ApplicationOptions options)
    {
        _logger = CreateLogger(options.LogPath);
        _logger.Information(Environment.CommandLine);
        _logger.Information("Initializing the application...");
        _client = new(this, options.PrivateKey);
        _isSeed = options.IsSeed;
        _container = new(this);
        _container.ComposeExportedValue<IApplication>(this);
        _container.ComposeExportedValue(this);
        _container.ComposeExportedValue<IServiceProvider>(this);
        _container.ComposeExportedValue(_client);
        _container.ComposeExportedValue<IClient>(_client);
        _container.ComposeExportedValue<ILogger>(_logger);
        _clientServiceContext = _container.GetValue<ClientServiceContext>();
        _clientServiceContext.EndPoint = options.EndPoint;
        _container.GetValue<IApplicationConfigurations>();
        _info = new()
        {
            EndPoint = EndPointUtility.ToString(_clientServiceContext.EndPoint),
            NodeEndPoint = options.NodeEndPoint is EndPoint nodeEndPoint ?
                EndPointUtility.ToString(nodeEndPoint) :
                string.Empty,
            LogPath = options.LogPath,
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

    public EndPoint EndPoint => _clientServiceContext.EndPoint;

    public ApplicationInfo Info => _info;

    public override ILogger Logger => _logger;

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
        _closeToken = await _clientServiceContext.StartAsync(cancellationToken: default);
        await base.OnStartAsync(cancellationToken);
        await AutoStartAsync(cancellationToken);
        _logger.Information("Started the application.");
    }

    protected override async ValueTask OnDisposeAsync()
    {
        _logger.Information("Disposing the application...");
        await base.OnDisposeAsync();
        await _clientServiceContext.CloseAsync(_closeToken, CancellationToken.None);
        await _container.DisposeAsync();
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
        if (_info.NodeEndPoint != string.Empty)
        {
            var clientOptions = new ClientOptions
            {
                NodeEndPoint = await GetNodeEndPointAsync(cancellationToken),
            };
            await _client.StartAsync(clientOptions, cancellationToken: cancellationToken);
        }

        async Task<EndPoint> GetNodeEndPointAsync(CancellationToken cancellationToken)
        {
            var nodeEndPoint = EndPointUtility.Parse(_info.NodeEndPoint);
            if (_isSeed == true)
            {
                return await SeedUtility.GetNodeEndPointAsync(nodeEndPoint, cancellationToken);
            }

            return nodeEndPoint;
        }
    }
}

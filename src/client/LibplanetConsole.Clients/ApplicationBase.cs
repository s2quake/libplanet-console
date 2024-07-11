using System.Collections;
using System.ComponentModel.Composition;
using System.Diagnostics;
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
    private readonly Task _waitForExitTask = Task.CompletedTask;
    private Guid _closeToken;

    protected ApplicationBase(ApplicationOptions options)
    {
        _logger = CreateLogger(options.LogPath);
        _logger.Debug(Environment.CommandLine);
        _logger.Debug("Application initializing...");
        _client = new(this, options);
        _isSeed = options.IsSeed;
        _container = new(this);
        _container.ComposeExportedValue<ILogger>(_logger);
        _container.ComposeExportedValue<IApplication>(this);
        _container.ComposeExportedValue(this);
        _container.ComposeExportedValue<IServiceProvider>(this);
        _container.ComposeExportedValue(_client);
        _container.ComposeExportedValue<IClient>(_client);
        _container.ComposeExportedValue<IBlockChain>(_client);
        _clientServiceContext = _container.GetValue<ClientServiceContext>();
        _clientServiceContext.EndPoint = options.EndPoint;
        _container.GetValue<IApplicationConfigurations>();
        _info = new()
        {
            EndPoint = _clientServiceContext.EndPoint,
            NodeEndPoint = options.NodeEndPoint,
            LogPath = options.LogPath,
        };
        ApplicationServices = new(_container.GetExportedValues<IApplicationService>());
        if (options.ParentProcessId != 0 &&
            Process.GetProcessById(options.ParentProcessId) is { } parentProcess)
        {
            _parentProcess = parentProcess;
            _waitForExitTask = WaitForExit(parentProcess, Cancel);
        }

        _logger.Debug("Application initialized.");
    }

    public override ApplicationServiceCollection ApplicationServices { get; }

    public AppEndPoint EndPoint => _clientServiceContext.EndPoint;

    public ApplicationInfo Info => _info;

    public override ILogger Logger => _logger;

    protected override bool CanClose => _parentProcess?.HasExited == true;

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

    protected override async Task OnRunAsync(CancellationToken cancellationToken)
    {
        _closeToken = await _clientServiceContext.StartAsync(cancellationToken: default);
        await base.OnRunAsync(cancellationToken);
        await AutoStartAsync(cancellationToken);
    }

    protected override async ValueTask OnDisposeAsync()
    {
        await base.OnDisposeAsync();
        await _clientServiceContext.CloseAsync(_closeToken, CancellationToken.None);
        await _container.DisposeAsync();
        await _waitForExitTask;
    }

    private static async Task WaitForExit(Process process, Action cancelAction)
    {
        await process.WaitForExitAsync();
        cancelAction.Invoke();
    }

    private static Logger CreateLogger(string logFilename)
    {
        var loggerConfiguration = new LoggerConfiguration();
        if (logFilename != string.Empty)
        {
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

    private async Task AutoStartAsync(CancellationToken cancellationToken)
    {
        if (_info.NodeEndPoint is { } nodeEndPoint)
        {
            var isSeed = _isSeed;
            _client.NodeEndPoint = await SeedUtility.GetNodeEndPointAsync(
                nodeEndPoint, isSeed, cancellationToken);
            await _client.StartAsync(cancellationToken);
        }
    }
}

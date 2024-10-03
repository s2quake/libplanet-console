using System.Collections;
using System.Diagnostics;
using LibplanetConsole.Client.Services;
using LibplanetConsole.Framework;
using LibplanetConsole.Framework.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace LibplanetConsole.Client;

public abstract class ApplicationBase : ApplicationFramework, IApplication
{
    private readonly ApplicationContainer _container;
    private readonly Client _client;
    private readonly ClientServiceContext _clientServiceContext;
    private readonly Process? _parentProcess;
    private readonly ApplicationInfo _info;
    private readonly ILogger _logger;
    private readonly Task _waitForExitTask = Task.CompletedTask;
    private readonly IServiceProvider _serviceProvider;
    private Guid _closeToken;

    protected ApplicationBase(ApplicationOptions options)
    {
        _logger = CreateLogger(GetType(), options.LogPath, string.Empty);
        _logger.Debug(Environment.CommandLine);
        _logger.Debug("Application initializing...");
        _client = new(this, options);
        _container = new(this);
        _container.AddSingleton(_logger);
        _container.AddSingleton(this);
        _container.AddSingleton<IApplication>(_ => this);
        _container.AddSingleton<IServiceProvider>(_ => this);
        _container.AddSingleton(_client);
        _container.AddSingleton<IClient>(_ => _client);
        _container.AddSingleton<IBlockChain>(_ => _client);
        _serviceProvider = _container.BuildServiceProvider();
        // _container.ComposeExportedValues(options.Components);
        // _clientServiceContext = _container<ClientServiceContext>();
        // _clientServiceContext.EndPoint = options.EndPoint;
        // _container.GetValue<IApplicationConfigurations>();
        _info = new()
        {
            EndPoint = _clientServiceContext.EndPoint,
            NodeEndPoint = options.NodeEndPoint,
            LogPath = options.LogPath,
        };
        // ApplicationServices = new(_container.GetExportedValues<IApplicationService>());
        if (options.ParentProcessId != 0 &&
            Process.GetProcessById(options.ParentProcessId) is { } parentProcess)
        {
            _parentProcess = parentProcess;
            _waitForExitTask = WaitForExit(parentProcess, Cancel);
        }

        _logger.Debug("Application initialized.");
    }

    public override ApplicationServiceCollection ApplicationServices { get; }

    public EndPoint EndPoint => _clientServiceContext.EndPoint;

    public ApplicationInfo Info => _info;

    public override ILogger Logger => _logger;

    protected override bool CanClose => _parentProcess?.HasExited == true;

    public override object? GetService(Type serviceType)
    {
        return _serviceProvider.GetService(serviceType);
        // var isMultiple = serviceType.IsGenericType &&
        //     serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>);

        // if (isMultiple == true)
        // {
        //     var itemType = serviceType.GenericTypeArguments[0];
        //     var contractName = AttributedModelServices.GetContractName(itemType);
        //     var items = _container.GetExportedValues<object?>(contractName);
        //     var listGenericType = typeof(List<>);
        //     var list = listGenericType.MakeGenericType(itemType);
        //     var ci = list.GetConstructor([typeof(int)]) ?? throw new UnreachableException();
        //     var instance = (IList)ci.Invoke([items.Count(),]);
        //     foreach (var item in items)
        //     {
        //         instance.Add(item);
        //     }

        //     return instance;
        // }
        // else
        // {
        //     var contractName = AttributedModelServices.GetContractName(serviceType);
        //     return _container.GetExportedValue<object?>(contractName);
        // }
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

    private async Task AutoStartAsync(CancellationToken cancellationToken)
    {
        if (_info.NodeEndPoint is { } nodeEndPoint)
        {
            _client.NodeEndPoint = nodeEndPoint;
            await _client.StartAsync(cancellationToken);
        }
    }
}

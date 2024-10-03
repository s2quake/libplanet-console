using System.Collections;
using System.Diagnostics;
using LibplanetConsole.Framework;
using LibplanetConsole.Framework.Extensions;
using LibplanetConsole.Node.Services;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace LibplanetConsole.Node;

public abstract class ApplicationBase : ApplicationFramework, IApplication
{
    // private readonly ApplicationContainer _container;
    private readonly Node _node;
    private readonly NodeContext _nodeContext;
    private readonly Process? _parentProcess;
    private readonly ApplicationInfo _info;
    private readonly ILogger _logger;
    private readonly Task _waitForExitTask = Task.CompletedTask;
    private readonly IServiceProvider _serviceProvider;
    private Guid _closeToken;

    protected ApplicationBase(IServiceProvider serviceProvider, ApplicationOptions options)
        : base(serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _logger = serviceProvider.GetRequiredService<ILogger>();
        _logger.Debug(Environment.CommandLine);
        _logger.Debug("Application initializing...");
        _node = serviceProvider.GetRequiredService<Node>();
        // _container = new(this);
        // _container.AddSingleton(_logger);
        // _container.AddSingleton<IApplication>(_ => this);
        // _container.AddSingleton(this);
        // _container.AddSingleton<IServiceProvider>(_ => this);
        // _container.AddSingleton(_node);
        // _container.AddSingleton<INode>(_ => _node);
        // _container.AddSingleton<IBlockChain>(_ => _node);
        // _container.AddSingleton<NodeContext>();
        // _container.ComposeExportedValues(options.Components);
        // _container.ComposeParts(options.Components);
        // _serviceProvider = _container.BuildServiceProvider();
        _nodeContext = _serviceProvider.GetRequiredService<NodeContext>();
        _nodeContext.EndPoint = options.EndPoint;
        _logger.Debug(options.EndPoint.ToString());
        // _container.GetValue<IApplicationConfigurations>();
        _info = new()
        {
            EndPoint = _nodeContext.EndPoint,
            SeedEndPoint = options.SeedEndPoint,
            StorePath = options.StorePath,
            LogPath = options.LogPath,
            ParentProcessId = options.ParentProcessId,
        };
        // ApplicationServices = new(_serviceProvider.GetServices<IApplicationService>());
        if (options.ParentProcessId != 0 &&
            Process.GetProcessById(options.ParentProcessId) is { } parentProcess)
        {
            _parentProcess = parentProcess;
            _waitForExitTask = WaitForExit(parentProcess, Cancel);
        }

        _logger.Debug("Application initialized.");
    }

    // public override ApplicationServiceCollection ApplicationServices { get; }

    public EndPoint EndPoint => _nodeContext.EndPoint;

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
        _closeToken = await _nodeContext.StartAsync(cancellationToken: default);
        await base.OnRunAsync(cancellationToken);
        await AutoStartAsync(cancellationToken);
    }

    protected override async ValueTask OnDisposeAsync()
    {
        await base.OnDisposeAsync();
        await _nodeContext.CloseAsync(_closeToken, cancellationToken: default);
        // await _container.DisposeAsync();
        await _waitForExitTask;
    }

    private static async Task WaitForExit(Process process, Action cancelAction)
    {
        await process.WaitForExitAsync();
        cancelAction.Invoke();
    }

    private async Task AutoStartAsync(CancellationToken cancellationToken)
    {
        if (_info.SeedEndPoint is { } seedEndPoint)
        {
            _node.SeedEndPoint = seedEndPoint;
            await _node.StartAsync(cancellationToken);
        }
    }
}

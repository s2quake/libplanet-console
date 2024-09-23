using System.Collections;
using System.ComponentModel.Composition;
using System.Diagnostics;
using LibplanetConsole.Common;
using LibplanetConsole.Frameworks;
using LibplanetConsole.Frameworks.Extensions;
using LibplanetConsole.Nodes.Services;
using Serilog;

namespace LibplanetConsole.Nodes;

public abstract class ApplicationBase : ApplicationFramework, IApplication
{
    private readonly ApplicationContainer _container;
    private readonly Node _node;
    private readonly NodeContext _nodeContext;
    private readonly Process? _parentProcess;
    private readonly ApplicationInfo _info;
    private readonly ILogger _logger;
    private readonly Task _waitForExitTask = Task.CompletedTask;
    private Guid _closeToken;

    protected ApplicationBase(ApplicationOptions options)
    {
        _logger = CreateLogger(GetType(), options.LogPath, options.LibraryLogPath);
        _logger.Debug(Environment.CommandLine);
        _logger.Debug("Application initializing...");
        _node = new Node(this, options, _logger);
        _container = new(this);
        _container.ComposeExportedValue(_logger);
        _container.ComposeExportedValue<IApplication>(this);
        _container.ComposeExportedValue(this);
        _container.ComposeExportedValue<IServiceProvider>(this);
        _container.ComposeExportedValue(_node);
        _container.ComposeExportedValue<INode>(_node);
        _container.ComposeExportedValue<IBlockChain>(_node);
        _container.ComposeExportedValues(options.Components);
        _container.ComposeParts(options.Components);
        _nodeContext = _container.GetValue<NodeContext>();
        _nodeContext.EndPoint = options.EndPoint;
        _logger.Debug(options.EndPoint.ToString());
        _container.GetValue<IApplicationConfigurations>();
        _info = new()
        {
            EndPoint = _nodeContext.EndPoint,
            SeedEndPoint = options.SeedEndPoint,
            StorePath = options.StorePath,
            LogPath = options.LogPath,
            ParentProcessId = options.ParentProcessId,
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

    public AppEndPoint EndPoint => _nodeContext.EndPoint;

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
        _closeToken = await _nodeContext.StartAsync(cancellationToken: default);
        await base.OnRunAsync(cancellationToken);
        await AutoStartAsync(cancellationToken);
    }

    protected override async ValueTask OnDisposeAsync()
    {
        await base.OnDisposeAsync();
        await _nodeContext.CloseAsync(_closeToken, cancellationToken: default);
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
        if (_info.SeedEndPoint is { } seedEndPoint)
        {
            _node.SeedEndPoint = seedEndPoint;
            await _node.StartAsync(cancellationToken);
        }
    }
}

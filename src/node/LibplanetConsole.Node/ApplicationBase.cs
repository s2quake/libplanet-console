using System.Diagnostics;
using LibplanetConsole.Framework;
using LibplanetConsole.Node.Services;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace LibplanetConsole.Node;

public abstract class ApplicationBase : ApplicationFramework, IApplication
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Node _node;
    private readonly Process? _parentProcess;
    private readonly ILogger _logger;
    private readonly ApplicationInfo _info;
    private readonly Task _waitForExitTask = Task.CompletedTask;
    private NodeContext? _nodeContext;
    private Guid _closeToken;

    protected ApplicationBase(IServiceProvider serviceProvider, ApplicationOptions options)
        : base(serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _logger = serviceProvider.GetRequiredService<ILogger>();
        _logger.Debug(Environment.CommandLine);
        _logger.Debug("Application initializing...");
        _node = serviceProvider.GetRequiredService<Node>();
        _info = new()
        {
            EndPoint = options.EndPoint,
            SeedEndPoint = options.SeedEndPoint,
            StorePath = options.StorePath,
            LogPath = options.LogPath,
            ParentProcessId = options.ParentProcessId,
        };
        if (options.ParentProcessId != 0 &&
            Process.GetProcessById(options.ParentProcessId) is { } parentProcess)
        {
            _parentProcess = parentProcess;
            _waitForExitTask = WaitForExit(parentProcess, Cancel);
        }

        _logger.Debug("Application initialized.");
    }

    public ApplicationInfo Info => _info;

    public override ILogger Logger => _logger;

    protected override bool CanClose => _parentProcess?.HasExited == true;

    public override object? GetService(Type serviceType)
        => _serviceProvider.GetService(serviceType);

    protected override async Task OnRunAsync(CancellationToken cancellationToken)
    {
        _nodeContext = _serviceProvider.GetRequiredService<NodeContext>();
        _nodeContext.EndPoint = _info.EndPoint;
        _closeToken = await _nodeContext.StartAsync(cancellationToken: default);
        await base.OnRunAsync(cancellationToken);
        await AutoStartAsync(cancellationToken);
    }

    protected override async ValueTask OnDisposeAsync()
    {
        await base.OnDisposeAsync();
        if (_nodeContext is not null)
        {
            await _nodeContext.CloseAsync(_closeToken, cancellationToken: default);
        }

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

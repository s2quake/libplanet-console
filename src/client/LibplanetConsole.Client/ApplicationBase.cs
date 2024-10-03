using System.Diagnostics;
using LibplanetConsole.Client.Services;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Framework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LibplanetConsole.Client;

public abstract class ApplicationBase : ApplicationFramework, IApplication
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Client _client;
    private readonly Process? _parentProcess;
    private readonly ApplicationInfo _info;
    private readonly ILogger _logger;
    private readonly Task _waitForExitTask = Task.CompletedTask;
    // private ClientServiceContext? _clientServiceContext;
    private Guid _closeToken;

    protected ApplicationBase(IServiceProvider serviceProvider, ApplicationOptions options)
        : base(serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _logger = serviceProvider.GetLogger<ApplicationBase>();
        _logger.LogDebug(Environment.CommandLine);
        _logger.LogDebug("Application initializing...");
        _client = serviceProvider.GetRequiredService<Client>();
        _info = new()
        {
            EndPoint = options.EndPoint,
            NodeEndPoint = options.NodeEndPoint,
            LogPath = options.LogPath,
        };
        if (options.ParentProcessId != 0 &&
            Process.GetProcessById(options.ParentProcessId) is { } parentProcess)
        {
            _parentProcess = parentProcess;
            _waitForExitTask = WaitForExit(parentProcess, Cancel);
        }

        _logger.LogDebug("Application initialized.");
    }

    public EndPoint EndPoint => _info.EndPoint;

    public ApplicationInfo Info => _info;

    protected override bool CanClose => _parentProcess?.HasExited == true;

    public override object? GetService(Type serviceType)
        => _serviceProvider.GetService(serviceType);

    protected override async Task OnRunAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("ClientServiceContext is starting: {EndPoint}", _info.EndPoint);
        _clientServiceContext = _serviceProvider.GetRequiredService<ClientServiceContext>();
        _clientServiceContext.EndPoint = _info.EndPoint;
        _closeToken = await _clientServiceContext.StartAsync(cancellationToken: default);
        _logger.LogDebug("ClientServiceContext is started: {EndPoint}", _info.EndPoint);
        await base.OnRunAsync(cancellationToken);
        await AutoStartAsync(cancellationToken);
    }

    protected override async ValueTask OnDisposeAsync()
    {
        await base.OnDisposeAsync();
        if (_clientServiceContext is not null)
        {
            _logger.LogDebug("ClientServiceContext is closing: {EndPoint}", _info.EndPoint);
            await _clientServiceContext.CloseAsync(_closeToken, CancellationToken.None);
            _clientServiceContext = null;
            _logger.LogDebug("ClientServiceContext is closed: {EndPoint}", _info.EndPoint);
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
        if (_info.NodeEndPoint is { } nodeEndPoint)
        {
            _client.NodeEndPoint = nodeEndPoint;
            _logger.LogDebug("Client auto-starting: {EndPoint}", nodeEndPoint);
            await _client.StartAsync(cancellationToken);
            _logger.LogDebug("Client auto-started: {EndPoint}", nodeEndPoint);
        }
    }
}

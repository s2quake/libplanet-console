using System.Diagnostics;
using LibplanetConsole.Client.Services;
using LibplanetConsole.Framework;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace LibplanetConsole.Client;

public abstract class ApplicationBase : ApplicationFramework, IApplication
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Client _client;
    private readonly ClientServiceContext _clientServiceContext;
    private readonly Process? _parentProcess;
    private readonly ApplicationInfo _info;
    private readonly ILogger _logger;
    private readonly Task _waitForExitTask = Task.CompletedTask;
    private Guid _closeToken;

    protected ApplicationBase(IServiceProvider serviceProvider, ApplicationOptions options)
        : base(serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _logger = CreateLogger(GetType(), options.LogPath, string.Empty);
        _logger.Debug(Environment.CommandLine);
        _logger.Debug("Application initializing...");
        _client = serviceProvider.GetRequiredService<Client>();
        _clientServiceContext = serviceProvider.GetRequiredService<ClientServiceContext>();
        _clientServiceContext.EndPoint = options.EndPoint;
        _info = new()
        {
            EndPoint = _clientServiceContext.EndPoint,
            NodeEndPoint = options.NodeEndPoint,
            LogPath = options.LogPath,
        };
        if (options.ParentProcessId != 0 &&
            Process.GetProcessById(options.ParentProcessId) is { } parentProcess)
        {
            _parentProcess = parentProcess;
            _waitForExitTask = WaitForExit(parentProcess, Cancel);
        }

        _logger.Debug("Application initialized.");
    }

    public EndPoint EndPoint => _clientServiceContext.EndPoint;

    public ApplicationInfo Info => _info;

    public override ILogger Logger => _logger;

    protected override bool CanClose => _parentProcess?.HasExited == true;

    public override object? GetService(Type serviceType)
        => _serviceProvider.GetService(serviceType);

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

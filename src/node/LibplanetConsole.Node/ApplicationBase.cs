// using System.Diagnostics;
// using LibplanetConsole.Common.Extensions;
// using LibplanetConsole.Framework;
// using LibplanetConsole.Node.Services;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Logging;

// namespace LibplanetConsole.Node;

// public abstract class ApplicationBase : ApplicationFramework, IApplication
// {
//     private readonly IServiceProvider _serviceProvider;
//     private readonly Node _node;
//     private readonly Process? _parentProcess;
//     private readonly ILogger<ApplicationBase> _logger;
//     private readonly ApplicationInfo _info;
//     private readonly Task _waitForExitTask = Task.CompletedTask;
//     // private NodeContext? _nodeContext;
//     private Guid _closeToken;

//     protected ApplicationBase(IServiceProvider serviceProvider, ApplicationOptions options)
//         : base(serviceProvider)
//     {
//         _serviceProvider = serviceProvider;
//         _logger = serviceProvider.GetLogger<ApplicationBase>();
//         _logger.LogDebug(Environment.CommandLine);
//         _logger.LogDebug("Application initializing...");
//         _node = serviceProvider.GetRequiredService<Node>();
//         _info = new()
//         {
//             EndPoint = options.EndPoint,
//             SeedEndPoint = options.SeedEndPoint,
//             StorePath = options.StorePath,
//             LogPath = options.LogPath,
//             ParentProcessId = options.ParentProcessId,
//         };
//         if (options.ParentProcessId != 0 &&
//             Process.GetProcessById(options.ParentProcessId) is { } parentProcess)
//         {
//             _parentProcess = parentProcess;
//             _waitForExitTask = WaitForExit(parentProcess, Cancel);
//         }

//         _logger.LogDebug("Application initialized.");
//     }

//     public ApplicationInfo Info => _info;

//     protected override bool CanClose => _parentProcess?.HasExited == true;

//     public override object? GetService(Type serviceType)
//         => _serviceProvider.GetService(serviceType);

//     protected override async Task OnRunAsync(CancellationToken cancellationToken)
//     {
//         _logger.LogDebug("NodeContext is starting: {EndPoint}", _info.EndPoint);
//         // _nodeContext = _serviceProvider.GetRequiredService<NodeContext>();
//         // _nodeContext.EndPoint = _info.EndPoint;
//         // _closeToken = await _nodeContext.StartAsync(cancellationToken: default);
//         _logger.LogDebug("NodeContext is started: {EndPoint}", _info.EndPoint);
//         await base.OnRunAsync(cancellationToken);
//     }

//     protected override async ValueTask OnDisposeAsync()
//     {
//         await base.OnDisposeAsync();
//         // if (_nodeContext is not null)
//         // {
//         //     _logger.LogDebug("NodeContext is closing: {EndPoint}", _info.EndPoint);
//         //     await _nodeContext.CloseAsync(_closeToken, cancellationToken: default);
//         //     _nodeContext = null;
//         //     _logger.LogDebug("NodeContext is closed: {EndPoint}", _info.EndPoint);
//         // }

//         await _waitForExitTask;
//     }

//     private static async Task WaitForExit(Process process, Action cancelAction)
//     {
//         await process.WaitForExitAsync();
//         cancelAction.Invoke();
//     }
// }

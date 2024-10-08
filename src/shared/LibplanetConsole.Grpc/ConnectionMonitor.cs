using Grpc.Core;
using LibplanetConsole.Common.Threading;

#if LIBPLANET_CONSOLE
namespace LibplanetConsole.Console.Grpc;
#elif LIBPLANET_NODE
namespace LibplanetConsole.Node.Grpc;
#elif LIBPLANET_CLIENT
namespace LibplanetConsole.Client.Grpc;
#else
#error "Either LIBPLANET_CONSOLE or LIBPLANET_NODE must be defined."
#endif

internal class ConnectionMonitor<T>(T client, Func<T, CancellationToken, Task> action)
    : RunTask
{
    public event EventHandler? Disconnected;

    public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(5);

    protected override async Task OnRunAsync(CancellationToken cancellationToken)
    {
        while (await TaskUtility.TryDelay(1, cancellationToken))
        {
            try
            {
                await action(client, cancellationToken);
            }
            catch (RpcException)
            {
                Disconnected?.Invoke(this, EventArgs.Empty);
                break;
            }
        }
    }
}

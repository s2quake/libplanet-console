#if LIBPLANET_CONSOLE || LIBPLANET_CLIENT
using Grpc.Core;
using LibplanetConsole.Common.Threading;

namespace LibplanetConsole.Grpc;

internal class ConnectionMonitor<T>(T client, Func<T, CancellationToken, Task> action)
    : RunTask
{
    public event EventHandler? Disconnected;

    public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(5);

    protected override async Task OnStartAsync(CancellationToken cancellationToken)
    {
        await action(client, cancellationToken);
    }

    protected override async Task OnRunAsync(CancellationToken cancellationToken)
    {
        while (await TaskUtility.TryDelay(Interval, cancellationToken))
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
#endif // LIBPLANET_CONSOLE || LIBPLANET_CLIENT

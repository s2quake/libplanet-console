namespace LibplanetConsole.Common.Threading;

public static class TaskUtility
{
    public static async Task<bool> TryDelay(
        int millisecondsDelay, CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(millisecondsDelay, cancellationToken);
            return true;
        }
        catch (TaskCanceledException)
        {
            return false;
        }
    }
}

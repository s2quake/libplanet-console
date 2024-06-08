namespace LibplanetConsole.Common.Threading;

public static class TaskUtility
{
    public static async Task<bool> Delay(int millisecondsDelay, CancellationToken cancellationToken)
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

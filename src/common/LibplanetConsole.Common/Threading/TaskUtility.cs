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

    public static async Task<bool> TryDelay(
        TimeSpan delay, CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(delay, cancellationToken);
            return true;
        }
        catch (TaskCanceledException)
        {
            return false;
        }
    }

    public static async Task<bool> TryWait(Task task)
    {
        try
        {
            await task;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}

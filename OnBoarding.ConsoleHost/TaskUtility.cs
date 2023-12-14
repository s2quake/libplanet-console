namespace OnBoarding.ConsoleHost;

static class TaskUtility
{
    public static async Task WaitIfAsync(Func<bool> func, CancellationToken cancellationToken)
    {
        while (func() == true)
        {
            try
            {
                await Task.Delay(1, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }
}

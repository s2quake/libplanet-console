namespace LibplanetConsole.Console.Extensions;

public static class ProcessBaseExtensions
{
    public static string RunWithResult(this ProcessBase @this)
    {
        using var outputCollector = new ProcessOutputCollector(@this);
        @this.Run();
        return outputCollector.Output;
    }

    public static string RunWithResult(this ProcessBase @this, int millisecondsDelay)
    {
        using var outputCollector = new ProcessOutputCollector(@this);
        @this.Run(millisecondsDelay);
        return outputCollector.Output;
    }

    public static async Task<string> RunWithResultAsync(
        this ProcessBase @this, CancellationToken cancellationToken)
    {
        using var outputCollector = new ProcessOutputCollector(@this);
        await @this.RunAsync(default);
        return outputCollector.Output;
    }
}

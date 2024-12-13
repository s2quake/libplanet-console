namespace LibplanetConsole.Client.Extensions;

internal static class IClientContentsExtensions
{
    public static async Task StartAsync(
        this IClientContent[] @this, CancellationToken cancellationToken)
    {
        for (var i = 0; i < @this.Length; i++)
        {
            await @this[i].StartAsync(cancellationToken);
        }
    }

    public static async Task StopAsync(
        this IClientContent[] @this, CancellationToken cancellationToken)
    {
        for (var i = @this.Length - 1; i >= 0; i--)
        {
            await @this[i].StopAsync(cancellationToken);
        }
    }
}

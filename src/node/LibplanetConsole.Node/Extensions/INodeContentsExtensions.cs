namespace LibplanetConsole.Node.Extensions;

internal static class INodeContentsExtensions
{
    public static async Task StartAsync(
        this INodeContent[] @this, CancellationToken cancellationToken)
    {
        for (var i = 0; i < @this.Length; i++)
        {
            await @this[i].StartAsync(cancellationToken);
        }
    }

    public static async Task StopAsync(
        this INodeContent[] @this, CancellationToken cancellationToken)
    {
        for (var i = @this.Length - 1; i >= 0; i--)
        {
            await @this[i].StopAsync(cancellationToken);
        }
    }
}

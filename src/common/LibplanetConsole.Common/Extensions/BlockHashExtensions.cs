namespace LibplanetConsole.Common.Extensions;

public static class BlockHashExtensions
{
    public static string ToShortString(this BlockHash @this)
        => @this.ToString()[..8];
}

namespace LibplanetConsole.Console.Extensions;

public static class IClientCollectionExtensions
{
    public static IClient GetClientOrCurrent(this IClientCollection @this, string address)
    {
        if (address == string.Empty)
        {
            return @this.Current ?? throw new InvalidOperationException("No client is selected.");
        }

        return @this[new Address(address)];
    }
}

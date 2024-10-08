namespace LibplanetConsole.Console.Extensions;

public static class INodeCollectionExtensions
{
    public static INode GetNodeOrCurrent(this INodeCollection @this, string address)
    {
        if (address == string.Empty)
        {
            return @this.Current ?? throw new InvalidOperationException("No node is selected.");
        }

        return @this[new Address(address)];
    }
}

using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Consoles.Extensions;

public static class IAddressableExtensions
{
    public static INode GetNode(this IAddressable @this)
    {
        if (@this is INode node)
        {
            return node;
        }

        if (@this is IClient client)
        {
            return client.GetService<INode>();
        }

        throw new InvalidOperationException("The addressable does not have a node.");
    }
}

using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Consoles.Extensions;

public static class IApplicationExtensions
{
    public static IClient GetClient(this IApplication @this, string address)
    {
        var clients = @this.GetService<IClientCollection>();
        if (address == string.Empty)
        {
            return clients.Current ?? throw new InvalidOperationException("No client is selected.");
        }

        return clients.Where(item => $"{item.Address}".StartsWith(address)).Single();
    }

    public static INode GetNode(this IApplication @this, string address)
    {
        var nodes = @this.GetService<INodeCollection>();
        if (address == string.Empty)
        {
            return nodes.Current ?? throw new InvalidOperationException("No node is selected.");
        }

        return nodes.Where(item => $"{item.Address}".StartsWith(address)).Single();
    }

    public static IAddressable GetAddressable(this IApplication @this, string address)
    {
        var clients = @this.GetService<IClientCollection>();
        var nodes = @this.GetService<INodeCollection>();
        return nodes.Concat<IAddressable>(clients)
                    .Where(item => $"{item.Address}".StartsWith(address))
                    .Single();
    }

    public static INode GetViewingNode(
        this IApplication @this, string address, string nodeAddress)
    {
        if (nodeAddress is not null)
        {
            return @this.GetNode(nodeAddress);
        }

        var addressable = GetAddressable(@this, address);
        if (addressable is INode node)
        {
            return node;
        }

        if (addressable is IClient client)
        {
            return GetNode(@this, AddressUtility.ToString(client.Info.NodeAddress));
        }

        throw new InvalidOperationException($"The address '{address}' is not a node nor a client.");
    }
}

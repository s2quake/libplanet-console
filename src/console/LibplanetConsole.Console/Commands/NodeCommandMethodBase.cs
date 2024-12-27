using JSSoft.Commands;
using LibplanetConsole.Console.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console.Commands;

public abstract class NodeCommandMethodBase : CommandMethodBase
{
    private readonly IServiceProvider _serviceProvider;

    protected NodeCommandMethodBase(IServiceProvider serviceProvider, string name)
        : base(name)
    {
        _serviceProvider = serviceProvider;
    }

    protected NodeCommandMethodBase(
        IServiceProvider serviceProvider, ICommand parentCommand, string name)
        : base(parentCommand, name)
    {
        _serviceProvider = serviceProvider;
    }

    protected NodeCommandMethodBase(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected NodeCommandMethodBase(
        IServiceProvider serviceProvider, ICommand parentCommand)
        : base(parentCommand)
    {
        _serviceProvider = serviceProvider;
    }

    [CommandProperty("node", 'N', InitValue = "")]
    [CommandSummary("Specifies the address of the node to use")]
    [CommandPropertyCompletion(nameof(GetNodeAddresses))]
    public string NodeAddress { get; set; } = string.Empty;

    protected INode GetNodeOrCurrent(string nodeAddress)
    {
        var nodes = _serviceProvider.GetRequiredService<INodeCollection>();
        var addresses = _serviceProvider.GetRequiredService<IAddressCollection>();
        return nodes.GetNodeOrCurrent(nodeAddress, addresses);
    }

    protected INode? GetNodeOrDefault(string nodeAddress)
    {
        if (nodeAddress == string.Empty)
        {
            return default;
        }

        var nodes = _serviceProvider.GetRequiredService<INodeCollection>();
        var addresses = _serviceProvider.GetRequiredService<IAddressCollection>();
        var address = addresses.ToAddress(nodeAddress);
        return nodes[address];
    }

    protected string[] GetNodeAddresses()
    {
        var nodes = _serviceProvider.GetRequiredService<INodeCollection>();
        return
        [
            .. nodes.Where(item => item.Alias != string.Empty).Select(item => item.Alias),
            .. nodes.Select(node => $"{node.Address}"),
        ];
    }

    protected Address GetAddress(string address)
    {
        if (address == string.Empty)
        {
            return default;
        }

        var addresses = _serviceProvider.GetRequiredService<IAddressCollection>();
        return addresses.ToAddress(address);
    }
}

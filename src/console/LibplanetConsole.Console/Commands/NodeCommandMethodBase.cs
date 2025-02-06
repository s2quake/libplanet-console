using JSSoft.Commands;
using LibplanetConsole.Alias;
using LibplanetConsole.Console.Extensions;

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

    [CommandProperty("current", 'C')]
    [CommandSummary("Specifies the address of the node to use")]
    [CommandPropertyCompletion(nameof(GetNodeAddresses))]
    public Address NodeAddress { get; set; }

    protected INode CurrentNode
    {
        get
        {
            var nodes = _serviceProvider.GetRequiredService<INodeCollection>();
            return nodes.GetNodeOrCurrent(NodeAddress);
        }
    }

    protected INode GetNodeOrCurrent(Address nodeAddress)
    {
        var nodes = _serviceProvider.GetRequiredService<INodeCollection>();
        return nodes.GetNodeOrCurrent(nodeAddress);
    }

    protected INode? GetNodeOrDefault(Address nodeAddress)
    {
        if (nodeAddress == default)
        {
            return default;
        }

        var nodes = _serviceProvider.GetRequiredService<INodeCollection>();
        return nodes[nodeAddress];
    }

    protected Address GetAddress(string address)
    {
        if (address == string.Empty)
        {
            return default;
        }

        var aliases = _serviceProvider.GetRequiredService<IAliasCollection>();
        return aliases.ToAddress(address);
    }

    protected string[] GetNodeAddresses() => GetAddresses(INode.Tag);

    protected string[] GetAddresses(params string[] tags)
    {
        var aliases = _serviceProvider.GetRequiredService<IAliasCollection>();
        return aliases.GetAddresses(tags);
    }
}

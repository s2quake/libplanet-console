using JSSoft.Commands;
using LibplanetConsole.Console.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console.Commands;

public abstract class NodeCommandBase : CommandBase
{
    private readonly IServiceProvider _serviceProvider;

    protected NodeCommandBase(IServiceProvider serviceProvider, string name)
        : base(name)
    {
        _serviceProvider = serviceProvider;
    }

    protected NodeCommandBase(
        IServiceProvider serviceProvider, ICommand parentCommand, string name)
        : base(parentCommand, name)
    {
        _serviceProvider = serviceProvider;
    }

    protected NodeCommandBase(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected NodeCommandBase(
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

    protected string[] GetNodeAddresses() => GetAddresses(INode.Tag);

    protected string[] GetAddresses(params string[] tags)
    {
        var addresses = _serviceProvider.GetRequiredService<IAddressCollection>();
        return addresses.GetAddresses(tags);
    }
}

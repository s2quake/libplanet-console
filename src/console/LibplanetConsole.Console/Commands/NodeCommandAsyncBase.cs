using JSSoft.Commands;
using LibplanetConsole.Alias;
using LibplanetConsole.Console.Extensions;

namespace LibplanetConsole.Console.Commands;

public abstract class NodeCommandAsyncBase : CommandAsyncBase
{
    private readonly IServiceProvider _serviceProvider;

    protected NodeCommandAsyncBase(IServiceProvider serviceProvider, string name)
        : base(name)
    {
        _serviceProvider = serviceProvider;
    }

    protected NodeCommandAsyncBase(
        IServiceProvider serviceProvider, ICommand parentCommand, string name)
        : base(parentCommand, name)
    {
        _serviceProvider = serviceProvider;
    }

    protected NodeCommandAsyncBase(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected NodeCommandAsyncBase(
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
        var aliases = _serviceProvider.GetRequiredService<IAliasCollection>();
        return aliases.GetAddresses(tags);
    }
}

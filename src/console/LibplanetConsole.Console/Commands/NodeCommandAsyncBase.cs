using JSSoft.Commands;
using LibplanetConsole.Console.Extensions;
using Microsoft.Extensions.DependencyInjection;

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

    [CommandProperty("node", 'N', InitValue = "")]
    [CommandSummary("Specifies the address of the node to use")]
    [CommandPropertyCompletion(nameof(GetNodeAddresses))]
    public string NodeAddress { get; set; } = string.Empty;

    protected INode Node
    {
        get
        {
            var nodes = _serviceProvider.GetRequiredService<INodeCollection>();
            var addresses = _serviceProvider.GetRequiredService<IAddressCollection>();
            return nodes.GetNodeOrCurrent(NodeAddress, addresses);
        }
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
}

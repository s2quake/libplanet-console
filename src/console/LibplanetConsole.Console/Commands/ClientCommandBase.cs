using JSSoft.Commands;
using LibplanetConsole.Console.Extensions;

namespace LibplanetConsole.Console.Commands;

public abstract class ClientCommandBase : CommandBase
{
    private readonly IServiceProvider _serviceProvider;

    protected ClientCommandBase(IServiceProvider serviceProvider, string name)
        : base(name)
    {
        _serviceProvider = serviceProvider;
    }

    protected ClientCommandBase(
        IServiceProvider serviceProvider, ICommand parentCommand, string name)
        : base(parentCommand, name)
    {
        _serviceProvider = serviceProvider;
    }

    protected ClientCommandBase(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected ClientCommandBase(
        IServiceProvider serviceProvider, ICommand parentCommand)
        : base(parentCommand)
    {
        _serviceProvider = serviceProvider;
    }

    [CommandProperty("current", 'C')]
    [CommandSummary("Specifies the address of the client to use")]
    [CommandPropertyCompletion(nameof(GetClientAddresses))]
    public Address ClientAddress { get; set; }

    protected IClient Client
    {
        get
        {
            var clients = _serviceProvider.GetRequiredService<IClientCollection>();
            return clients.GetClientOrCurrent(ClientAddress);
        }
    }

    protected string[] GetClientAddresses()
    {
        var addresses = _serviceProvider.GetRequiredService<IAddressCollection>();
        return addresses.GetAddresses(IClient.Tag);
    }
}

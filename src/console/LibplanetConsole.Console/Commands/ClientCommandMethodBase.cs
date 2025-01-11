using JSSoft.Commands;
using LibplanetConsole.Console.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console.Commands;

public abstract class ClientCommandMethodBase : CommandMethodBase
{
    private readonly IServiceProvider _serviceProvider;

    protected ClientCommandMethodBase(IServiceProvider serviceProvider, string name)
        : base(name)
    {
        _serviceProvider = serviceProvider;
    }

    protected ClientCommandMethodBase(
        IServiceProvider serviceProvider, ICommand parentCommand, string name)
        : base(parentCommand, name)
    {
        _serviceProvider = serviceProvider;
    }

    protected ClientCommandMethodBase(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected ClientCommandMethodBase(
        IServiceProvider serviceProvider, ICommand parentCommand)
        : base(parentCommand)
    {
        _serviceProvider = serviceProvider;
    }

    [CommandProperty("current", 'C')]
    [CommandSummary("Specifies the address of the client to use")]
    [CommandPropertyCompletion(nameof(GetClientAddresses))]
    public Address ClientAddress { get; set; }

    public IClient CurrentClient
    {
        get
        {
            var clients = _serviceProvider.GetRequiredService<IClientCollection>();
            return clients.GetClientOrCurrent(ClientAddress);
        }
    }

    protected IClient GetClientOrCurrent(Address clientAddress)
    {
        var clients = _serviceProvider.GetRequiredService<IClientCollection>();
        return clients.GetClientOrCurrent(clientAddress);
    }

    protected IClient? GetClientOrDefault(Address clientAddress)
    {
        if (clientAddress == default)
        {
            return default;
        }

        var clients = _serviceProvider.GetRequiredService<IClientCollection>();
        return clients[clientAddress];
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

    protected string[] GetClientAddresses() => GetAddresses(IClient.Tag);

    protected string[] GetAddresses(params string[] tags)
    {
        var addresses = _serviceProvider.GetRequiredService<IAddressCollection>();
        return addresses.GetAddresses(tags);
    }
}

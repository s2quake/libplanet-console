using JSSoft.Commands;
using LibplanetConsole.Console.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console.Commands;

public abstract class ClientCommandAsyncBase : CommandAsyncBase
{
    private readonly IServiceProvider _serviceProvider;

    protected ClientCommandAsyncBase(IServiceProvider serviceProvider, string name)
        : base(name)
    {
        _serviceProvider = serviceProvider;
    }

    protected ClientCommandAsyncBase(
        IServiceProvider serviceProvider, ICommand parentCommand, string name)
        : base(parentCommand, name)
    {
        _serviceProvider = serviceProvider;
    }

    protected ClientCommandAsyncBase(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected ClientCommandAsyncBase(
        IServiceProvider serviceProvider, ICommand parentCommand)
        : base(parentCommand)
    {
        _serviceProvider = serviceProvider;
    }

    [CommandProperty("client", 'C', InitValue = "")]
    [CommandSummary("Specifies the address of the client to use")]
    [CommandPropertyCompletion(nameof(GetClientAddresses))]
    public string ClientAddress { get; set; } = string.Empty;

    protected IClient Client
    {
        get
        {
            var clients = _serviceProvider.GetRequiredService<IClientCollection>();
            var addresses = _serviceProvider.GetRequiredService<IAddressCollection>();
            var address = addresses.ToAddress(ClientAddress);
            return clients.GetClientOrCurrent(address);
        }
    }

    protected string[] GetClientAddresses()
    {
        var clients = _serviceProvider.GetRequiredService<IClientCollection>();
        return
        [
            .. clients.Where(item => item.Alias != string.Empty).Select(item => item.Alias),
            .. clients.Select(client => $"{client.Address}"),
        ];
    }
}

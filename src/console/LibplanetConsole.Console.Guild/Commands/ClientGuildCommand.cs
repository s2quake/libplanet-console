using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Console.Commands;
using LibplanetConsole.Console.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console.Guild.Commands;

internal sealed partial class ClientGuildCommand(
    ClientCommand clientCommand, IClientCollection clients, INodeCollection nodes)
    : CommandMethodBase(clientCommand, "guild")
{
    [CommandPropertyRequired(DefaultValue = "")]
    [CommandSummary("The address of the client. If not specified, the current client is used.")]
    [CommandPropertyCompletion(nameof(GetClientAddresses))]
    public static string Address { get; set; } = string.Empty;

    [CommandMethod]
    public async Task CreateAsync(
        [CommandParameterCompletion(nameof(GetNodeAddresses))]
        Address validatorAddress,
        CancellationToken cancellationToken)
    {
        var client = clients.GetClientOrCurrent(Address);
        var guild = client.GetRequiredKeyedService<IClientGuild>(IClient.Key);
        await guild.CreateAsync(validatorAddress, cancellationToken);
    }

    [CommandMethod]
    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
        var client = clients.GetClientOrCurrent(Address);
        var guild = client.GetRequiredKeyedService<IClientGuild>(IClient.Key);
        await guild.DeleteAsync(cancellationToken);
    }

    [CommandMethod]
    public async Task JoinAsync(Address guildAddress, CancellationToken cancellationToken)
    {
        var client = clients.GetClientOrCurrent(Address);
        var guild = client.GetRequiredKeyedService<IClientGuild>(IClient.Key);
        await guild.JoinAsync(guildAddress, cancellationToken);
    }

    [CommandMethod]
    public async Task LeaveAsync(CancellationToken cancellationToken)
    {
        var client = clients.GetClientOrCurrent(Address);
        var guild = client.GetRequiredKeyedService<IClientGuild>(IClient.Key);
        await guild.LeaveAsync(cancellationToken);
    }

    [CommandMethod]
    public async Task BanAsync(Address memberAddress, CancellationToken cancellationToken)
    {
        var client = clients.GetClientOrCurrent(Address);
        var guild = client.GetRequiredKeyedService<IClientGuild>(IClient.Key);
        await guild.BanAsync(memberAddress, cancellationToken);
    }

    [CommandMethod]
    public async Task UnbanAsync(Address memberAddress, CancellationToken cancellationToken)
    {
        var client = clients.GetClientOrCurrent(Address);
        var guild = client.GetRequiredKeyedService<IClientGuild>(IClient.Key);
        await guild.UnbanAsync(memberAddress, cancellationToken);
    }

    [CommandMethod]
    public async Task ClaimAsync(CancellationToken cancellationToken)
    {
        var client = clients.GetClientOrCurrent(Address);
        var guild = client.GetRequiredKeyedService<IClientGuild>(IClient.Key);
        await guild.ClaimAsync(cancellationToken);
    }

    [CommandMethod]
    public async Task InfoAsync(CancellationToken cancellationToken)
    {
        var client = clients.GetClientOrCurrent(Address);
        var guild = client.GetRequiredKeyedService<IClientGuild>(IClient.Key);
        var memberAddress = client.Address;
        var info = await guild.GetInfoAsync(memberAddress, cancellationToken);
        await Out.WriteLineAsJsonAsync(info, cancellationToken);
    }

    private string[] GetNodeAddresses() => [.. nodes.Select(node => node.Address.ToString())];

    private string[] GetClientAddresses()
        => [.. clients.Select(client => client.Address.ToString())];
}

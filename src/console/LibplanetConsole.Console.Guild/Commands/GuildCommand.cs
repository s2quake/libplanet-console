using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Alias;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Console.Guild.Commands;

[CommandSummary("Provides commands for the guild service.")]
[Category(nameof(Guild))]
internal sealed class GuildCommand(
    IConsole console,
    IGuild guild,
    IAliasCollection alises) : CommandMethodBase
{
    [CommandMethod]
    public async Task CreateAsync(
        [CommandParameterCompletion(nameof(GetNodeAddresses))]
        Address nodeAddress,
        CancellationToken cancellationToken)
    {
        await guild.CreateAsync(nodeAddress, cancellationToken);
        var info = await guild.GetInfoAsync(console.Address, cancellationToken);
        await Out.WriteLineAsJsonAsync(info, cancellationToken);
    }

    [CommandMethod]
    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
        await guild.DeleteAsync(cancellationToken);
    }

    [CommandMethod]
    public async Task JoinAsync(Address guildAddress, CancellationToken cancellationToken)
    {
        await guild.JoinAsync(guildAddress, cancellationToken);
    }

    [CommandMethod]
    public async Task LeaveAsync(CancellationToken cancellationToken)
    {
        await guild.LeaveAsync(cancellationToken);
    }

    [CommandMethod]
    public async Task BanAsync(
        [CommandParameterCompletion(nameof(GetMemberAddresses))]
        Address memberAddress,
        CancellationToken cancellationToken)
    {
        await guild.BanAsync(memberAddress, cancellationToken);
    }

    [CommandMethod]
    public async Task UnbanAsync(
        [CommandParameterCompletion(nameof(GetMemberAddresses))]
        Address memberAddress,
        CancellationToken cancellationToken)
    {
        await guild.UnbanAsync(memberAddress, cancellationToken);
    }

    [CommandMethod]
    public async Task ClaimAsync(CancellationToken cancellationToken)
    {
        await guild.ClaimAsync(cancellationToken);
    }

    [CommandMethod]
    public async Task InfoAsync(
        [CommandParameterCompletion(nameof(GetMemberAddresses))]
        Address address = default,
        CancellationToken cancellationToken = default)
    {
        var actualAddress = address == default ? console.Address : address;
        var info = await guild.GetInfoAsync(actualAddress, cancellationToken);
        await Out.WriteLineAsJsonAsync(info, cancellationToken);
    }

    private string[] GetNodeAddresses() => alises.GetAddresses(INode.Tag);

    private string[] GetMemberAddresses() => alises.GetAddresses(IConsole.Tag, IClient.Tag);
}

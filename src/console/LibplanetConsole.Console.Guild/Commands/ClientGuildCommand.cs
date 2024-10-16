using System.ComponentModel;
using System.Text;
using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Console.Commands;
using LibplanetConsole.Guild;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console.Guild.Commands;

internal sealed class ClientGuildCommand(
    ClientCommand clientCommand, IApplication application)
    : CommandMethodBase(clientCommand, "guild")
{
    [CommandPropertyRequired(DefaultValue = "")]
    [CommandSummary("The address of the client. If not specified, the current client is used.")]
    public static string Address { get; set; } = string.Empty;

    [CommandMethod]
    [CommandSummary("Create new guild.")]
    [CommandMethodProperty(nameof(Address))]
    [Category("Guild")]
    public async Task NewAsync(CancellationToken cancellationToken = default)
    {
        var client = application.GetClient(Address);
        var guildClient = client.GetRequiredService<IGuildClientContent>();
        var options = new CreateGuildOptions
        {
            Name = "Guild",
        };
        await guildClient.CreateAsync(options.Sign(client), cancellationToken);
        var guildAddress = await guildClient.GetGuildAsync(client.Address, default);
        var message = GuildEventMessage.CreatedMessage(guildAddress);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
    [CommandSummary("Delete the guild.")]
    [CommandMethodProperty(nameof(Address))]
    [Category("Guild")]
    public async Task DeleteAsync(CancellationToken cancellationToken = default)
    {
        var client = application.GetClient(Address);
        var guildClient = client.GetRequiredService<IGuildClientContent>();
        var options = new DeleteGuildOptions
        {
        };
        var guildAddress = await guildClient.DeleteAsync(options.Sign(client), cancellationToken);
        var message = GuildEventMessage.DeletedMessage(guildAddress);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
    [CommandSummary("Request to join the guild.")]
    [CommandMethodProperty(nameof(Address))]
    [Category("Guild")]
    public async Task RequestJoinAsync(
        Address guildAddress, CancellationToken cancellationToken = default)
    {
        var client = application.GetClient(Address);
        var guildClient = client.GetRequiredService<IGuildClientContent>();
        var options = new RequestJoinOptions
        {
            GuildAddress = guildAddress,
        };
        var memberAddress = client.Address;
        var message = GuildEventMessage.RequestedJoinMessage(guildAddress, memberAddress);
        await guildClient.RequestJoinAsync(options.Sign(client), cancellationToken);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
    [CommandSummary("Cancel the request to join the guild.")]
    [CommandMethodProperty(nameof(Address))]
    [Category("Guild")]
    public async Task CancelJoinAsync(
        CancellationToken cancellationToken = default)
    {
        var client = application.GetClient(Address);
        var guildClient = client.GetRequiredService<IGuildClientContent>();
        var options = new CancelJoinOptions
        {
        };
        var memberAddress = client.Address;
        var message = GuildEventMessage.CanceledJoinMessage(memberAddress);
        await guildClient.CancelJoinAsync(options.Sign(client), cancellationToken);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
    [CommandSummary("Request to join the guild.")]
    [CommandMethodProperty(nameof(Address))]
    [Category("Guild")]
    public async Task AcceptJoinAsync(
        Address memberAddress, CancellationToken cancellationToken = default)
    {
        var client = application.GetClient(Address);
        var guildClient = client.GetRequiredService<IGuildClientContent>();
        var options = new AcceptJoinOptions
        {
            MemberAddress = memberAddress,
        };
        var guildAddress = guildClient.Info.Address;
        var message = GuildEventMessage.AcceptedJoinMessage(guildAddress, memberAddress);
        await guildClient.AcceptJoinAsync(options.Sign(client), cancellationToken);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
    [CommandSummary("Request to join the guild.")]
    [CommandMethodProperty(nameof(Address))]
    [Category("Guild")]
    public async Task RejectJoinAsync(
        Address memberAddress, CancellationToken cancellationToken = default)
    {
        var client = application.GetClient(Address);
        var guildClient = client.GetRequiredService<IGuildClientContent>();
        var options = new RejectJoinOptions
        {
            MemberAddress = memberAddress,
        };
        var guildAddress = guildClient.Info.Address;
        var message = GuildEventMessage.RejectedJoinMessage(guildAddress, memberAddress);
        await guildClient.RejectJoinAsync(options.Sign(client), cancellationToken);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
    [CommandSummary("Leave the guild.")]
    [CommandMethodProperty(nameof(Address))]
    [Category("Guild")]
    public async Task LeaveAsync(CancellationToken cancellationToken)
    {
        var client = application.GetClient(Address);
        var guildClient = client.GetRequiredService<IGuildClientContent>();
        var options = new LeaveGuildOptions
        {
        };
        var guildAddress = guildClient.Info.Address;
        var message = GuildEventMessage.LeftMessage(guildAddress, guildAddress);
        await guildClient.QuitAsync(options.Sign(client), cancellationToken);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
    [CommandSummary("Ban the member.")]
    [CommandMethodProperty(nameof(Address))]
    [Category("Guild")]
    public async Task BanMemberAsync(
        Address memberAddress, CancellationToken cancellationToken)
    {
        var client = application.GetClient(Address);
        var guildClient = client.GetRequiredService<IGuildClientContent>();
        var options = new BanMemberOptions
        {
            MemberAddress = memberAddress,
        };
        var guildAddress = guildClient.Info.Address;
        var message = GuildEventMessage.BannedMessage(guildAddress, memberAddress);
        await guildClient.BanMemberAsync(options.Sign(client), cancellationToken);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
    [CommandSummary("Unban the member.")]
    [CommandMethodProperty(nameof(Address))]
    [Category("Guild")]
    public async Task UnbanMemberAsync(
        Address memberAddress, CancellationToken cancellationToken)
    {
        var client = application.GetClient(Address);
        var guildClient = client.GetRequiredService<IGuildClientContent>();
        var options = new UnbanMemberOptions
        {
            MemberAddress = memberAddress,
        };
        var guildAddress = guildClient.Info.Address;
        var message = GuildEventMessage.UnbannedMessage(guildAddress, memberAddress);
        await guildClient.UnbanMemberAsync(options.Sign(client), cancellationToken);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
    [CommandSummary("List the member.")]
    [CommandMethodProperty(nameof(Address))]
    [Category("Guild")]
    public async Task ListMembersAsync(
        Address guildAddress = default, CancellationToken cancellationToken = default)
    {
        var client = application.GetClient(Address);
        var guildClient = client.GetRequiredService<IGuildClientContent>();
        var members = await guildClient.GetGuildMembersAsync(GetGuildAddress(), cancellationToken);
        var sb = new StringBuilder();
        for (var i = 0; i < members.Length; i++)
        {
            sb.AppendLine($"[{i:d2}] {members[i]}");
        }

        await Out.WriteLineAsync(sb.ToString());

        Address GetGuildAddress()
            => guildAddress == default ? guildClient.Info.Address : guildAddress;
    }
}

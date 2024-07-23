using System.ComponentModel;
using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Guild;

namespace LibplanetConsole.Consoles.Guild.Commands;

[Export(typeof(ICommand))]
[PartialCommand]
[method: ImportingConstructor]
internal sealed class ClientCommand(IApplication application) : CommandMethodBase
{
    [CommandPropertyRequired(DefaultValue = "")]
    [CommandSummary("The address of the client. If not specified, the current client is used.")]
    public static string Address { get; set; } = string.Empty;

    [CommandMethod("new-guild")]
    [CommandSummary("Create new guild.")]
    [CommandMethodProperty(nameof(Address))]
    [Category("Guild")]
    public async Task NewAsync(CancellationToken cancellationToken = default)
    {
        var client = application.GetClient(Address);
        var guildClient = client.GetService<IGuildClientContent>();
        var options = new CreateGuildOptions
        {
            Name = "Guild",
        };
        await guildClient.CreateAsync(options.Sign(client), cancellationToken);
    }

    [CommandMethod("rm-guild")]
    [CommandSummary("Delete the guild.")]
    [CommandMethodProperty(nameof(Address))]
    [Category("Guild")]
    public async Task DeleteAsync(CancellationToken cancellationToken = default)
    {
        var client = application.GetClient(Address);
        var guildClient = client.GetService<IGuildClientContent>();
        var options = new DeleteGuildOptions
        {
        };
        await guildClient.DeleteAsync(options.Sign(client), cancellationToken);
    }

    [CommandMethod]
    [CommandSummary("Request to join the guild.")]
    [CommandMethodProperty(nameof(Address))]
    [Category("Guild")]
    public async Task RequestJoinAsync(
        string guildAddress, CancellationToken cancellationToken = default)
    {
        var client = application.GetClient(Address);
        var guildClient = client.GetService<IGuildClientContent>();
        var options = new RequestJoinOptions
        {
            GuildAddress = AppAddress.Parse(guildAddress),
        };
        await guildClient.RequestJoinAsync(options, cancellationToken);
    }

    [CommandMethod]
    [CommandSummary("Cancel the request to join the guild.")]
    [CommandMethodProperty(nameof(Address))]
    [Category("Guild")]
    public async Task CancelJoinAsync(
        CancellationToken cancellationToken = default)
    {
        var client = application.GetClient(Address);
        var guildClient = client.GetService<IGuildClientContent>();
        var options = new CancelJoinOptions
        {
        };
        await guildClient.CancelJoinAsync(options, cancellationToken);
    }

    [CommandMethod]
    [CommandSummary("Request to join the guild.")]
    [CommandMethodProperty(nameof(Address))]
    [Category("Guild")]
    public async Task AcceptJoinAsync(
        string memberAddress, CancellationToken cancellationToken = default)
    {
        var client = application.GetClient(Address);
        var guildClient = client.GetService<IGuildClientContent>();
        var options = new AcceptJoinOptions
        {
            MemberAddress = AppAddress.Parse(memberAddress),
        };
        await guildClient.AcceptJoinAsync(options, cancellationToken);
    }

    [CommandMethod]
    [CommandSummary("Request to join the guild.")]
    [CommandMethodProperty(nameof(Address))]
    [Category("Guild")]
    public async Task RejectJoinAsync(
        string memberAddress, CancellationToken cancellationToken = default)
    {
        var client = application.GetClient(Address);
        var guildClient = client.GetService<IGuildClientContent>();
        var options = new RejectJoinOptions
        {
            MemberAddress = AppAddress.Parse(memberAddress),
        };
        await guildClient.RejectJoinAsync(options, cancellationToken);
    }

    [CommandMethod("leave-guild")]
    [CommandSummary("Leave the guild.")]
    [CommandMethodProperty(nameof(Address))]
    [Category("Guild")]
    public async Task LeaveAsync(CancellationToken cancellationToken)
    {
        var client = application.GetClient(Address);
        var guildClient = client.GetService<IGuildClientContent>();
        var options = new LeaveGuildOptions
        {
        };
        await guildClient.QuitAsync(options.Sign(client), cancellationToken);
    }

    [CommandMethod("ban-member")]
    [CommandSummary("Ban the member.")]
    [CommandMethodProperty(nameof(Address))]
    [Category("Guild")]
    public async Task BanMemberAsync(
        string memberAddress, CancellationToken cancellationToken)
    {
        var client = application.GetClient(Address);
        var guildClient = client.GetService<IGuildClientContent>();
        var options = new BanMemberOptions
        {
            MemberAddress = AppAddress.Parse(memberAddress),
        };
        await guildClient.BanMemberAsync(options.Sign(client), cancellationToken);
    }

    [CommandMethod("unban-member")]
    [CommandSummary("Unban the member.")]
    [CommandMethodProperty(nameof(Address))]
    [Category("Guild")]
    public async Task UnbanMemberAsync(string memberAddress, CancellationToken cancellationToken)
    {
        var client = application.GetClient(Address);
        var guildClient = client.GetService<IGuildClientContent>();
        var options = new UnbanMemberOptions
        {
            MemberAddress = AppAddress.Parse(memberAddress),
        };
        await guildClient.UnbanMemberAsync(options.Sign(client), cancellationToken);
    }
}

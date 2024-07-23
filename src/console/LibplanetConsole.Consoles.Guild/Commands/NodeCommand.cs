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
internal sealed class NodeCommand(IApplication application) : CommandMethodBase
{
    [CommandPropertyRequired(DefaultValue = "")]
    [CommandSummary("The address of the node. If not specified, the current node is used.")]
    public static string Address { get; set; } = string.Empty;

    [CommandMethod("new-guild")]
    [CommandSummary("Create new guild.")]
    [CommandMethodProperty(nameof(Address))]
    [Category("Guild")]
    public async Task NewAsync(CancellationToken cancellationToken = default)
    {
        var node = application.GetNode(Address);
        var guildNode = node.GetService<IGuildNodeContent>();
        var options = new CreateGuildOptions
        {
            Name = "Guild",
        };
        await guildNode.CreateAsync(options.Sign(node), cancellationToken);
    }

    [CommandMethod("rm-guild")]
    [CommandSummary("Delete the guild.")]
    [CommandMethodProperty(nameof(Address))]
    [Category("Guild")]
    public async Task DeleteAsync(CancellationToken cancellationToken = default)
    {
        var node = application.GetNode(Address);
        var guildNode = node.GetService<IGuildNodeContent>();
        var options = new DeleteGuildOptions
        {
        };
        await guildNode.DeleteAsync(options.Sign(node), cancellationToken);
    }

    [CommandMethod]
    [CommandSummary("Request to join the guild.")]
    [CommandMethodProperty(nameof(Address))]
    [Category("Guild")]
    public async Task RequestJoinAsync(
        string guildAddress, CancellationToken cancellationToken = default)
    {
        var node = application.GetNode(Address);
        var guildNode = node.GetService<IGuildNodeContent>();
        var options = new RequestJoinOptions
        {
            GuildAddress = AppAddress.Parse(guildAddress),
        };
        await guildNode.RequestJoinAsync(options, cancellationToken);
    }

    [CommandMethod]
    [CommandSummary("Cancel the request to join the guild.")]
    [CommandMethodProperty(nameof(Address))]
    [Category("Guild")]
    public async Task CancelJoinAsync(
        CancellationToken cancellationToken = default)
    {
        var node = application.GetNode(Address);
        var guildNode = node.GetService<IGuildNodeContent>();
        var options = new CancelJoinOptions
        {
        };
        await guildNode.CancelJoinAsync(options, cancellationToken);
    }

    [CommandMethod]
    [CommandSummary("Request to join the guild.")]
    [CommandMethodProperty(nameof(Address))]
    [Category("Guild")]
    public async Task AcceptJoinAsync(
        string memberAddress, CancellationToken cancellationToken = default)
    {
        var node = application.GetNode(Address);
        var guildNode = node.GetService<IGuildNodeContent>();
        var options = new AcceptJoinOptions
        {
            MemberAddress = AppAddress.Parse(memberAddress),
        };
        await guildNode.AcceptJoinAsync(options, cancellationToken);
    }

    [CommandMethod]
    [CommandSummary("Request to join the guild.")]
    [CommandMethodProperty(nameof(Address))]
    [Category("Guild")]
    public async Task RejectJoinAsync(
        string memberAddress, CancellationToken cancellationToken = default)
    {
        var node = application.GetNode(Address);
        var guildNode = node.GetService<IGuildNodeContent>();
        var options = new RejectJoinOptions
        {
            MemberAddress = AppAddress.Parse(memberAddress),
        };
        await guildNode.RejectJoinAsync(options, cancellationToken);
    }

    [CommandMethod("leave-guild")]
    [CommandSummary("Leave the guild.")]
    [CommandMethodProperty(nameof(Address))]
    [Category("Guild")]
    public async Task LeaveAsync(CancellationToken cancellationToken)
    {
        var node = application.GetNode(Address);
        var guildNode = node.GetService<IGuildNodeContent>();
        var options = new LeaveGuildOptions
        {
        };
        await guildNode.QuitAsync(options.Sign(node), cancellationToken);
    }

    [CommandMethod("ban-member")]
    [CommandSummary("Ban the member.")]
    [CommandMethodProperty(nameof(Address))]
    [Category("Guild")]
    public async Task BanMemberAsync(
        string memberAddress, CancellationToken cancellationToken)
    {
        var node = application.GetNode(Address);
        var guildNode = node.GetService<IGuildNodeContent>();
        var options = new BanMemberOptions
        {
            MemberAddress = AppAddress.Parse(memberAddress),
        };
        await guildNode.BanMemberAsync(options.Sign(node), cancellationToken);
    }

    [CommandMethod("unban-member")]
    [CommandSummary("Unban the member.")]
    [CommandMethodProperty(nameof(Address))]
    [Category("Guild")]
    public async Task UnbanMemberAsync(string memberAddress, CancellationToken cancellationToken)
    {
        var node = application.GetNode(Address);
        var guildNode = node.GetService<IGuildNodeContent>();
        var options = new UnbanMemberOptions
        {
            MemberAddress = AppAddress.Parse(memberAddress),
        };
        await guildNode.UnbanMemberAsync(options.Sign(node), cancellationToken);
    }
}

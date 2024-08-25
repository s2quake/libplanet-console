using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Text;
using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Consoles.Commands;
using LibplanetConsole.Guild;

namespace LibplanetConsole.Consoles.Guild.Commands;

[Export(typeof(ICommand))]
[method: ImportingConstructor]
internal sealed class NodeGuildCommand(
    NodeCommand nodeCommand, IApplication application)
    : CommandMethodBase(nodeCommand, "guild")
{
    [CommandPropertyRequired(DefaultValue = "")]
    [CommandSummary("The address of the node. If not specified, the current node is used.")]
    public static string Address { get; set; } = string.Empty;

    [CommandMethod]
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
        var guildAddress = await guildNode.GetGuildAsync(node.Address, default);
        var message = GuildEventMessage.CreatedMessage(guildAddress);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
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
        var guildAddress = await guildNode.DeleteAsync(options.Sign(node), cancellationToken);
        var message = GuildEventMessage.DeletedMessage(guildAddress);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
    [CommandSummary("Request to join the guild.")]
    [CommandMethodProperty(nameof(Address))]
    [Category("Guild")]
    public async Task RequestJoinAsync(
        AppAddress guildAddress, CancellationToken cancellationToken = default)
    {
        var node = application.GetNode(Address);
        var guildNode = node.GetService<IGuildNodeContent>();
        var options = new RequestJoinOptions
        {
            GuildAddress = guildAddress,
        };
        var memberAddress = node.Address;
        var message = GuildEventMessage.RequestedJoinMessage(guildAddress, memberAddress);
        await guildNode.RequestJoinAsync(options.Sign(node), cancellationToken);
        await Out.WriteLineAsync(message);
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
        var memberAddress = node.Address;
        var message = GuildEventMessage.CanceledJoinMessage(memberAddress);
        await guildNode.CancelJoinAsync(options.Sign(node), cancellationToken);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
    [CommandSummary("Request to join the guild.")]
    [CommandMethodProperty(nameof(Address))]
    [Category("Guild")]
    public async Task AcceptJoinAsync(
        AppAddress memberAddress, CancellationToken cancellationToken = default)
    {
        var node = application.GetNode(Address);
        var guildNode = node.GetService<IGuildNodeContent>();
        var options = new AcceptJoinOptions
        {
            MemberAddress = memberAddress,
        };
        var guildAddress = guildNode.Info.Address;
        var message = GuildEventMessage.AcceptedJoinMessage(guildAddress, memberAddress);
        await guildNode.AcceptJoinAsync(options.Sign(node), cancellationToken);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
    [CommandSummary("Request to join the guild.")]
    [CommandMethodProperty(nameof(Address))]
    [Category("Guild")]
    public async Task RejectJoinAsync(
        AppAddress memberAddress, CancellationToken cancellationToken = default)
    {
        var node = application.GetNode(Address);
        var guildNode = node.GetService<IGuildNodeContent>();
        var options = new RejectJoinOptions
        {
            MemberAddress = memberAddress,
        };
        var guildAddress = guildNode.Info.Address;
        var message = GuildEventMessage.RejectedJoinMessage(guildAddress, memberAddress);
        await guildNode.RejectJoinAsync(options.Sign(node), cancellationToken);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
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
        var guildAddress = guildNode.Info.Address;
        var message = GuildEventMessage.LeftMessage(guildAddress, guildAddress);
        await guildNode.QuitAsync(options.Sign(node), cancellationToken);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
    [CommandSummary("Ban the member.")]
    [CommandMethodProperty(nameof(Address))]
    [Category("Guild")]
    public async Task BanMemberAsync(
        AppAddress memberAddress, CancellationToken cancellationToken)
    {
        var node = application.GetNode(Address);
        var guildNode = node.GetService<IGuildNodeContent>();
        var options = new BanMemberOptions
        {
            MemberAddress = memberAddress,
        };
        var guildAddress = guildNode.Info.Address;
        var message = GuildEventMessage.BannedMessage(guildAddress, memberAddress);
        await guildNode.BanMemberAsync(options.Sign(node), cancellationToken);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
    [CommandSummary("Unban the member.")]
    [CommandMethodProperty(nameof(Address))]
    [Category("Guild")]
    public async Task UnbanMemberAsync(
        AppAddress memberAddress, CancellationToken cancellationToken)
    {
        var node = application.GetNode(Address);
        var guildNode = node.GetService<IGuildNodeContent>();
        var options = new UnbanMemberOptions
        {
            MemberAddress = memberAddress,
        };
        var guildAddress = guildNode.Info.Address;
        var message = GuildEventMessage.UnbannedMessage(guildAddress, memberAddress);
        await guildNode.UnbanMemberAsync(options.Sign(node), cancellationToken);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
    [CommandSummary("List the member.")]
    [CommandMethodProperty(nameof(Address))]
    [Category("Guild")]
    public async Task ListMembersAsync(
        AppAddress guildAddress = default, CancellationToken cancellationToken = default)
    {
        var node = application.GetNode(Address);
        var guildNode = node.GetService<IGuildNodeContent>();
        var members = await guildNode.GetGuildMembersAsync(GetGuildAddress(), cancellationToken);
        var sb = new StringBuilder();
        for (var i = 0; i < members.Length; i++)
        {
            sb.AppendLine($"[{i:d2}] {members[i]}");
        }

        await Out.WriteLineAsync(sb.ToString());

        AppAddress GetGuildAddress()
            => guildAddress == default ? guildNode.Info.Address : guildAddress;
    }
}

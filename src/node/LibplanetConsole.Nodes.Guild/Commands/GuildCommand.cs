using System.ComponentModel.Composition;
using System.Text;
using Bencodex.Types;
using JSSoft.Commands;
using Libplanet.Blockchain;
using Libplanet.Crypto;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Guild;
using Nekoyume;
using Nekoyume.Model.Guild;
using Nekoyume.TypedAddress;

namespace LibplanetConsole.Nodes.Guild.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Provides commands for the guild service.")]
[method: ImportingConstructor]
internal sealed class GuildCommand(INode node, IGuildNode guildNode) : CommandMethodBase
{
    [CommandMethod]
    [CommandSummary("Create new guild.")]
    public async Task NewAsync(CancellationToken cancellationToken = default)
    {
        var options = new CreateGuildOptions
        {
            Name = "Guild",
        };
        var guildAddress = node.Address;
        var message = GuildEventMessage.CreatedMessage(guildAddress);
        await guildNode.CreateAsync(options, cancellationToken);
        await Out.WriteLineAsJsonAsync(message);
    }

    [CommandMethod(Aliases = ["rm"])]
    [CommandSummary("Delete the guild.")]
    public async Task DeleteAsync(CancellationToken cancellationToken = default)
    {
        var options = new DeleteGuildOptions
        {
        };
        var guildAddress = node.Address;
        var message = GuildEventMessage.DeletedMessage(guildAddress);
        await guildNode.DeleteAsync(options, cancellationToken);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
    [CommandSummary("Request to join the guild.")]
    public async Task RequestJoinAsync(
        string guildAddress, CancellationToken cancellationToken = default)
    {
        var guildAddress1 = AppAddress.Parse(guildAddress);
        var options = new RequestJoinOptions
        {
            GuildAddress = guildAddress1,
        };
        var memberAddress = node.Address;
        var message = GuildEventMessage.RequestedJoinMessage(guildAddress1, memberAddress);
        await guildNode.RequestJoinAsync(options, cancellationToken);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
    [CommandSummary("Cancel the request to join the guild.")]
    public async Task CancelJoinAsync(
        CancellationToken cancellationToken = default)
    {
        var options = new CancelJoinOptions
        {
        };
        var memberAddress = node.Address;
        var message = GuildEventMessage.CancelledJoinMessage(memberAddress);
        await guildNode.CancelJoinAsync(options, cancellationToken);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
    [CommandSummary("Request to join the guild.")]
    public async Task AcceptJoinAsync(
        string memberAddress, CancellationToken cancellationToken = default)
    {
        var memberAddress1 = AppAddress.Parse(memberAddress);
        var options = new AcceptJoinOptions
        {
            MemberAddress = memberAddress1,
        };
        var guildAddress = node.Address;
        var message = GuildEventMessage.AcceptedJoinMessage(guildAddress, memberAddress1);
        await guildNode.AcceptJoinAsync(options, cancellationToken);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
    [CommandSummary("Request to join the guild.")]
    public async Task RejectJoinAsync(
        string memberAddress, CancellationToken cancellationToken = default)
    {
        var memberAddress1 = AppAddress.Parse(memberAddress);
        var options = new RejectJoinOptions
        {
            MemberAddress = memberAddress1,
        };
        var guildAddress = node.Address;
        var message = GuildEventMessage.RejectedJoinMessage(guildAddress, memberAddress1);
        await guildNode.RejectJoinAsync(options, cancellationToken);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
    [CommandSummary("Leave the guild.")]
    public async Task LeaveAsync(CancellationToken cancellationToken)
    {
        var options = new LeaveGuildOptions
        {
        };
        var guildAddress = node.Address;
        var message = GuildEventMessage.LeftMessage(guildAddress, guildAddress);
        await guildNode.QuitAsync(options, cancellationToken);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
    [CommandSummary("Ban the member.")]
    public async Task BanMemberAsync(
        string memberAddress, CancellationToken cancellationToken)
    {
        var memberAddress1 = AppAddress.Parse(memberAddress);
        var options = new BanMemberOptions
        {
            MemberAddress = memberAddress1,
        };
        var guildAddress = node.Address;
        var message = GuildEventMessage.BannedMessage(guildAddress, memberAddress1);
        await guildNode.BanMemberAsync(options, cancellationToken);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
    [CommandSummary("Unban the member.")]
    public async Task UnbanMemberAsync(
        string memberAddress, CancellationToken cancellationToken)
    {
        var memberAddress1 = AppAddress.Parse(memberAddress);
        var options = new UnbanMemberOptions
        {
            MemberAddress = memberAddress1,
        };
        var guildAddress = node.Address;
        var message = GuildEventMessage.UnbannedMessage(guildAddress, memberAddress1);
        await guildNode.UnbanMemberAsync(options, cancellationToken);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
    [CommandSummary("List the member.")]
    public async Task ListMembersAsync(string guildAddress, CancellationToken cancellationToken)
    {
        var blockChain = node.GetService<BlockChain>();
        var trie = blockChain.GetWorldState().GetAccountState(Addresses.GuildParticipant).Trie;
        IEnumerable<(Address, GuildParticipant)> guildParticipants = trie.IterateValues()
            .Where(pair => pair.Value is List)
            .Select(pair => (
                new Address(Encoding.UTF8.GetString(Convert.FromHexString(pair.Path.Hex))),
                new GuildParticipant((List)pair.Value)));
        var filtered = guildParticipants
            .Where(pair => pair.Item2.GuildAddress == new GuildAddress(guildAddress));

        await Out.WriteLineAsync($"Members:");
        foreach (var (address, guildParticipant) in filtered)
        {
            await Out.WriteLineAsync(
                $"  - {address} at {guildParticipant.GuildAddress.ToString()}");
        }
    }
}

using JSSoft.Terminals;

namespace LibplanetConsole.Guild;

internal static class GuildEventMessage
{
    public static string CreatedMessage(Address guildAddress)
    {
        var tsb = new TerminalStringBuilder();
        tsb.Append($"Guild ");
        tsb.Foreground = TerminalColorType.Yellow;
        tsb.Append($"{guildAddress}");
        tsb.ResetOptions();
        tsb.Append($" created.");
        tsb.AppendEnd();
        return tsb.ToString();
    }

    public static string DeletedMessage(Address guildAddress)
    {
        var tsb = new TerminalStringBuilder();
        tsb.Append($"Guild ");
        tsb.Foreground = TerminalColorType.Yellow;
        tsb.Append($"{guildAddress}");
        tsb.ResetOptions();
        tsb.Append($" deleted.");
        tsb.AppendEnd();
        return tsb.ToString();
    }

    public static string RequestedJoinMessage(Address guildAddress, Address memberAddress)
    {
        var tsb = new TerminalStringBuilder();
        tsb.Append("Member ");
        tsb.Foreground = TerminalColorType.Cyan;
        tsb.Append($"{memberAddress}");
        tsb.ResetOptions();
        tsb.Append($" requested to join the guild ");
        tsb.Foreground = TerminalColorType.Yellow;
        tsb.Append($"{guildAddress}");
        tsb.ResetOptions();
        tsb.Append(".");
        tsb.AppendEnd();
        return tsb.ToString();
    }

    public static string CanceledJoinMessage(Address memberAddress)
    {
        var tsb = new TerminalStringBuilder();
        tsb.Append("Member ");
        tsb.Foreground = TerminalColorType.Cyan;
        tsb.Append($"{memberAddress}");
        tsb.ResetOptions();
        tsb.Append($" canceled the request to join the guild.");
        tsb.AppendEnd();
        return tsb.ToString();
    }

    public static string AcceptedJoinMessage(Address guildAddress, Address memberAddress)
    {
        var tsb = new TerminalStringBuilder();
        tsb.Append("Guild ");
        tsb.Foreground = TerminalColorType.Yellow;
        tsb.Append($"{guildAddress}");
        tsb.ResetOptions();
        tsb.Append($" accepted the request to join by ");
        tsb.Foreground = TerminalColorType.Cyan;
        tsb.Append($"{memberAddress}");
        tsb.ResetOptions();
        tsb.Append(".");
        tsb.AppendEnd();
        return tsb.ToString();
    }

    public static string RejectedJoinMessage(Address guildAddress, Address memberAddress)
    {
        var tsb = new TerminalStringBuilder();
        tsb.Append("Guild ");
        tsb.Foreground = TerminalColorType.Yellow;
        tsb.Append($"{guildAddress}");
        tsb.ResetOptions();
        tsb.Append($" rejected the request to join by ");
        tsb.Foreground = TerminalColorType.Cyan;
        tsb.Append($"{memberAddress}");
        tsb.ResetOptions();
        tsb.Append(".");
        tsb.AppendEnd();
        return tsb.ToString();
    }

    public static string LeftMessage(Address guildAddress, Address memberAddress)
    {
        var tsb = new TerminalStringBuilder();
        tsb.Append("Member ");
        tsb.Foreground = TerminalColorType.Cyan;
        tsb.Append($"{memberAddress}");
        tsb.ResetOptions();
        tsb.Append($" left the guild ");
        tsb.Foreground = TerminalColorType.Yellow;
        tsb.Append($"{guildAddress}");
        tsb.ResetOptions();
        tsb.Append(".");
        tsb.AppendEnd();
        return tsb.ToString();
    }

    public static string BannedMessage(Address guildAddress, Address memberAddress)
    {
        var tsb = new TerminalStringBuilder();
        tsb.Append("Guild ");
        tsb.Foreground = TerminalColorType.Yellow;
        tsb.Append($"{guildAddress}");
        tsb.ResetOptions();
        tsb.Append($" banned member ");
        tsb.Foreground = TerminalColorType.Cyan;
        tsb.Append($"{memberAddress}");
        tsb.ResetOptions();
        tsb.Append(".");
        tsb.AppendEnd();
        return tsb.ToString();
    }

    public static string UnbannedMessage(Address guildAddress, Address memberAddress)
    {
        var tsb = new TerminalStringBuilder();
        tsb.Append("Guild ");
        tsb.Foreground = TerminalColorType.Yellow;
        tsb.Append($"{guildAddress}");
        tsb.ResetOptions();
        tsb.Append($" unbanned member ");
        tsb.Foreground = TerminalColorType.Cyan;
        tsb.Append($"{memberAddress}");
        tsb.ResetOptions();
        tsb.Append(".");
        tsb.AppendEnd();
        return tsb.ToString();
    }
}

using JSSoft.Terminals;
using LibplanetConsole.Common;

namespace LibplanetConsole.Guild;

internal static class GuildEventMessage
{
    public static string CreatedMessage(AppAddress guildAddress)
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

    public static string DeletedMessage(AppAddress guildAddress)
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

    public static string RequestedJoinMessage(AppAddress guildAddress, AppAddress memberAddress)
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

    public static string CanceledJoinMessage(AppAddress memberAddress)
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

    public static string AcceptedJoinMessage(AppAddress guildAddress, AppAddress memberAddress)
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

    public static string RejectedJoinMessage(AppAddress guildAddress, AppAddress memberAddress)
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

    public static string LeftMessage(AppAddress guildAddress, AppAddress memberAddress)
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

    public static string BannedMessage(AppAddress guildAddress, AppAddress memberAddress)
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

    public static string UnbannedMessage(AppAddress guildAddress, AppAddress memberAddress)
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

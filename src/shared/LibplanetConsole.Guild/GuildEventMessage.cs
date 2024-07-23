using JSSoft.Terminals;
using LibplanetConsole.Common;

namespace LibplanetConsole.Guild;

internal static class GuildEventMessage
{
    public static object CreatedMessage(AppAddress guildAddress)
    {
        var tsb = new TerminalStringBuilder();
        tsb.Append($"Guild created: ");
        tsb.Foreground = TerminalColorType.Yellow;
        tsb.Append($"{guildAddress}");
        tsb.ResetOptions();
        tsb.AppendEnd();
        return tsb.ToString();
    }

    public static string DeletedMessage(AppAddress guildAddress)
    {
        var tsb = new TerminalStringBuilder();
        tsb.Append($"Guild deleted: ");
        tsb.Foreground = TerminalColorType.Yellow;
        tsb.Append($"{guildAddress}");
        tsb.ResetOptions();
        tsb.AppendEnd();
        return tsb.ToString();
    }

    public static string RequestedJoinMessage(AppAddress guildAddress, AppAddress memberAddress)
    {
        var tsb = new TerminalStringBuilder();
        tsb.Append($"Requested to join the guild: ");
        tsb.Foreground = TerminalColorType.Yellow;
        tsb.Append($"{guildAddress}");
        tsb.ResetOptions();
        tsb.Append($" by ");
        tsb.Foreground = TerminalColorType.Cyan;
        tsb.Append($"{memberAddress}");
        tsb.ResetOptions();
        tsb.AppendEnd();
        return tsb.ToString();
    }

    public static string CancelledJoinMessage(AppAddress memberAddress)
    {
        var tsb = new TerminalStringBuilder();
        tsb.Append($"Cancelled the request to join the guild by ");
        tsb.Foreground = TerminalColorType.Cyan;
        tsb.Append($"{memberAddress}");
        tsb.ResetOptions();
        tsb.AppendEnd();
        return tsb.ToString();
    }

    public static string AcceptedJoinMessage(AppAddress guildAddress, AppAddress memberAddress)
    {
        var tsb = new TerminalStringBuilder();
        tsb.Append($"Accepted the request to join the guild: ");
        tsb.Foreground = TerminalColorType.Yellow;
        tsb.Append($"{guildAddress}");
        tsb.ResetOptions();
        tsb.Append($" by ");
        tsb.Foreground = TerminalColorType.Cyan;
        tsb.Append($"{memberAddress}");
        tsb.ResetOptions();
        tsb.AppendEnd();
        return tsb.ToString();
    }

    public static string RejectedJoinMessage(AppAddress guildAddress, AppAddress memberAddress)
    {
        var tsb = new TerminalStringBuilder();
        tsb.Append($"Rejected the request to join the guild: ");
        tsb.Foreground = TerminalColorType.Yellow;
        tsb.Append($"{guildAddress}");
        tsb.ResetOptions();
        tsb.Append($" by ");
        tsb.Foreground = TerminalColorType.Cyan;
        tsb.Append($"{memberAddress}");
        tsb.ResetOptions();
        tsb.AppendEnd();
        return tsb.ToString();
    }

    public static string LeftMessage(AppAddress guildAddress, AppAddress memberAddress)
    {
        var tsb = new TerminalStringBuilder();
        tsb.Append($"Left the guild: ");
        tsb.Foreground = TerminalColorType.Yellow;
        tsb.Append($"{guildAddress}");
        tsb.ResetOptions();
        tsb.Append($" by ");
        tsb.Foreground = TerminalColorType.Cyan;
        tsb.Append($"{memberAddress}");
        tsb.ResetOptions();
        tsb.AppendEnd();
        return tsb.ToString();
    }

    public static string BannedMessage(AppAddress guildAddress, AppAddress memberAddress)
    {
        var tsb = new TerminalStringBuilder();
        tsb.Append($"Banned the member: ");
        tsb.Foreground = TerminalColorType.Cyan;
        tsb.Append($"{memberAddress}");
        tsb.ResetOptions();
        tsb.Append($" from ");
        tsb.Foreground = TerminalColorType.Yellow;
        tsb.Append($"{guildAddress}");
        tsb.ResetOptions();
        tsb.AppendEnd();
        return tsb.ToString();
    }

    public static string UnbannedMessage(AppAddress guildAddress, AppAddress memberAddress)
    {
        var tsb = new TerminalStringBuilder();
        tsb.Append($"Unbanned the member: ");
        tsb.Foreground = TerminalColorType.Cyan;
        tsb.Append($"{memberAddress}");
        tsb.ResetOptions();
        tsb.Append($" from ");
        tsb.Foreground = TerminalColorType.Yellow;
        tsb.Append($"{guildAddress}");
        tsb.ResetOptions();
        tsb.AppendEnd();
        return tsb.ToString();
    }
}

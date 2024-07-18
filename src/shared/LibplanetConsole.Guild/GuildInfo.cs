using System.Diagnostics.CodeAnalysis;
using Bencodex.Types;
using LibplanetConsole.Common;

namespace LibplanetConsole.Guild;

public readonly record struct GuildInfo
{
    public GuildInfo(IValue value)
    {
        if (value is not List list)
        {
            throw new ArgumentException("Invalid value type.", nameof(value));
        }

        var guild = new Nekoyume.Model.Guild.Guild(list);
        Address = (AppAddress)guild.GuildMaster;
    }

    public bool IsRunning { get; init; }

    public AppAddress Address { get; init; }

    public static bool TryParse(IValue value, [MaybeNullWhen(false)] out GuildInfo guildInfo)
    {
        try
        {
            guildInfo = new GuildInfo(value);
            return true;
        }
        catch
        {
            guildInfo = default;
            return false;
        }
    }
}

using Bencodex.Types;

namespace LibplanetConsole.Executable.Games.Serializations.Extensions;

static class EnumerableExtensions
{
    public static List ToBencodex(this MonsterInfo[] @this)
    {
        return @this.Aggregate(List.Empty, (l, n) => l.Add(n.ToBencodex()));
    }

    public static List ToBencodex(this SkillInfo[] @this)
    {
        return @this.Aggregate(List.Empty, (l, n) => l.Add(n.ToBencodex()));
    }
}

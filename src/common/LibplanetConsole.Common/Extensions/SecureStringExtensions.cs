using System.Security;

namespace LibplanetConsole.Common.Extensions;

internal static class SecureStringExtensions
{
    public static void AppendString(this SecureString @this, string @string)
    {
        foreach (var item in @string)
        {
            @this.AppendChar(item);
        }
    }
}

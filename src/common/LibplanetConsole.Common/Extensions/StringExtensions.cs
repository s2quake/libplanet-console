namespace LibplanetConsole.Common.Extensions;

public static class StringExtensions
{
    public static string Fallback(this string @this, string fallback)
    {
        if (@this == string.Empty)
        {
            if (fallback == string.Empty)
            {
                throw new ArgumentException("Both strings are empty.");
            }

            return fallback;
        }

        return @this;
    }
}

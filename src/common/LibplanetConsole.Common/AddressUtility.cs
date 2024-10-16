namespace LibplanetConsole.Common;

public static class AddressUtility
{
     public static Address ParseOrFallback(string text, Address fallback)
        => text == string.Empty ? fallback : new(text);
}

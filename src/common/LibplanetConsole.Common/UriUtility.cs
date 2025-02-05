namespace LibplanetConsole.Common;

public static class UriUtility
{
    public static Uri? ParseOrDefault(string text)
        => Uri.TryCreate(text, UriKind.Absolute, out var uri) ? uri : null;

    public static Uri ParseOrFallback(string text, Uri fallback)
        => text == string.Empty ? fallback : new Uri(text);

    public static Uri GetLocalHost(int port) => new($"http://localhost:{port}");
}

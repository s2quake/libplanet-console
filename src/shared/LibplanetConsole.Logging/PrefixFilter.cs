namespace LibplanetConsole.Logging;

internal record class PrefixFilter(string Name, string Prefix)
    : SourceContextFilter(Name, e => e.StartsWith(Prefix))
{
}

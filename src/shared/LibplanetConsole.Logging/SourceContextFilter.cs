using Serilog.Events;

namespace LibplanetConsole.Logging;

internal record class SourceContextFilter(string Name, Func<string, bool> Predicate)
    : LoggingFilter(Name, e => Test(e, Predicate))
{
    private static bool Test(LogEvent e, Func<string, bool> predicate)
    {
        if (e.Properties.TryGetValue("SourceContext", out var propertyValue) is false)
        {
            return false;
        }

        if (propertyValue is not ScalarValue scalarValue)
        {
            return false;
        }

        if (scalarValue.Value is not string value)
        {
            return false;
        }

        return predicate(value);
    }
}

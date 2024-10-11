using Serilog.Events;

namespace LibplanetConsole.Logging;

internal record class LoggingFilter(string Name, Func<LogEvent, bool> Filter)
{
}

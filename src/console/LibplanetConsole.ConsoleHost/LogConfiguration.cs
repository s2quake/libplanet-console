using System.ComponentModel.Composition;
using JSSoft.Configurations;
using LibplanetConsole.Frameworks;
using Serilog;
using Serilog.Events;

namespace LibplanetConsole.ConsoleHost;

[Export(typeof(IApplicationConfiguration))]
[ConfigurationName("log")]
internal sealed class LogConfiguration : IApplicationConfiguration
{
    public LogConfiguration()
    {
        Log.Logger = new LoggerConfiguration().Filter.ByIncludingOnly(Predicate)
                                              .WriteTo.Console()
                                              .CreateLogger();
    }

    [ConfigurationProperty("visible")]
    public bool IsVisible { get; set; }

    // [ConfigurationProperty]
    // public LogEventLevel Level { get; set; } = LogEventLevel.Error;
    private bool Predicate(LogEvent logEvent)
    {
        if (IsVisible != true)
        {
            return false;
        }

        // if (logEvent.Level < Level)
        //     return false;
        return true;
    }
}

using System.ComponentModel.Composition;
using JSSoft.Library;
using Serilog;
using Serilog.Events;

namespace OnBoarding.ConsoleHost;

[Export(typeof(IApplicationConfiguration))]
sealed class LogConfiguration : IApplicationConfiguration
{
    public LogConfiguration()
    {
        Log.Logger = new LoggerConfiguration().Filter.ByIncludingOnly(Predicate)
                                              .WriteTo.Console()
                                              .CreateLogger();
    }

    [ConfigurationProperty]
    public bool IsVisible { get; set; }

    // [ConfigurationProperty]
    // public LogEventLevel Level { get; set; } = LogEventLevel.Error;

    private bool Predicate(LogEvent logEvent)
    {
        if (IsVisible == false)
            return false;
        // if (logEvent.Level < Level)
        //     return false;
        return true;
    }
}

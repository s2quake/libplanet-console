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
                                              .MinimumLevel.Error()
                                              .WriteTo.Console()
                                              .CreateLogger();
    }

    [ConfigurationProperty]
    public bool IsLogVisible { get; set; }

    private bool Predicate(LogEvent logEvent)
    {
        return IsLogVisible == true;
    }
}

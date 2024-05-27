using System.ComponentModel.Composition;
using JSSoft.Configurations;
using LibplanetConsole.Frameworks;
using Serilog;
using Serilog.Events;

namespace LibplanetConsole.Consoles.Executable;

[Export(typeof(IApplicationConfiguration))]
[ConfigurationName("log")]
internal sealed class LogConfiguration : IApplicationConfiguration
{
    [ImportingConstructor]
    public LogConfiguration(ILogger logger)
    {
        Log.Logger = new LoggerConfiguration().WriteTo.Logger(logger)
                                              .WriteTo.Logger(WriteToConsole)
                                              .CreateLogger();

        void WriteToConsole(LoggerConfiguration loggerConfiguration)
        {
            loggerConfiguration.Filter.ByIncludingOnly(Predicate).WriteTo.Console();
        }
    }

    [ConfigurationProperty("visible")]
    public bool IsVisible { get; set; }

    private bool Predicate(LogEvent logEvent)
    {
        if (IsVisible != true)
        {
            return false;
        }

        return true;
    }
}

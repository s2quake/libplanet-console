using Serilog;
using Serilog.Events;

namespace LibplanetConsole.Logging;

public static class LoggingExtensions
{
    public const string AppSourceContext = "LibplanetConsole";

    public static IServiceCollection AddLogging(
        this IServiceCollection @this, string logPath, string libraryLogPath)
    {
        var loggerConfiguration = new LoggerConfiguration();
        loggerConfiguration = loggerConfiguration.MinimumLevel.Debug();
        if (logPath != string.Empty)
        {
            loggerConfiguration = loggerConfiguration
                .WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(e => IsApplicationLog(e))
                    .WriteTo.File(logPath));
        }

        if (libraryLogPath != string.Empty)
        {
            loggerConfiguration = loggerConfiguration
                .WriteTo.Logger(lc => lc
                    .Filter.ByExcluding(e => IsApplicationLog(e))
                    .WriteTo.File(libraryLogPath));
        }

        Log.Logger = loggerConfiguration.CreateLogger();
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            Log.Logger.Fatal(e.ExceptionObject as Exception, "Unhandled exception occurred.");
        };

        @this.AddSingleton<ILoggerFactory, LoggerFactory>();
        @this.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddSerilog();
        });

        return @this;
    }

    private static bool IsApplicationLog(LogEvent e)
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

        return value.StartsWith(AppSourceContext);
    }
}

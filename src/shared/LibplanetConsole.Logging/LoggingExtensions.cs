using Serilog;

namespace LibplanetConsole.Logging;

internal static class LoggingExtensions
{
    public static IServiceCollection AddLogging(
        this IServiceCollection @this, string logPath, string name, params LoggingFilter[] filters)
    {
        var loggerConfiguration = new LoggerConfiguration();
        var logFilename = Path.Combine(logPath, name);
        loggerConfiguration = loggerConfiguration.MinimumLevel.Debug();
        loggerConfiguration = loggerConfiguration
            .WriteTo.Logger(lc => lc.WriteTo.File(logFilename));

        foreach (var filter in filters)
        {
            var filename = Path.Combine(logPath, filter.Name);
            loggerConfiguration = loggerConfiguration
                .WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(filter.Filter)
                    .WriteTo.File(filename));
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
}

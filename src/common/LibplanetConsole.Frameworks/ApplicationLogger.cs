// Line must be no longer than 100 characters
#pragma warning disable MEN002
using Serilog;

namespace LibplanetConsole.Frameworks;

public static class ApplicationLogger
{
    private static ApplicationFramework? app;

    public static ILogger Logger => app?.Logger ??
        throw new InvalidOperationException("Application is not set.");

    public static void Debug(string messageTemplate)
        => Logger.Debug(messageTemplate: messageTemplate);

    public static void Debug<T>(string messageTemplate, T propertyValue)
        => Logger.Debug(messageTemplate, propertyValue);

    public static void Debug<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        => Logger.Debug(messageTemplate, propertyValue0, propertyValue1);

    public static void Debug<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        => Logger.Debug(messageTemplate, propertyValue0, propertyValue1, propertyValue2);

    public static void Debug(string messageTemplate, params object[] propertyValues)
        => Logger.Debug(messageTemplate, propertyValues: propertyValues);

    public static void Debug(Exception exception, string messageTemplate)
        => Logger.Debug(exception, messageTemplate);

    public static void Debug<T>(Exception exception, string messageTemplate, T propertyValue)
        => Logger.Debug(exception, messageTemplate, propertyValue);

    public static void Debug<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        => Logger.Debug(exception, messageTemplate, propertyValue0, propertyValue1);

    public static void Debug<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        => Logger.Debug(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);

    public static void Debug(Exception exception, string messageTemplate, params object[] propertyValues)
        => Logger.Debug(exception, messageTemplate, propertyValues: propertyValues);

    public static void Information(string messageTemplate)
        => Logger.Information(messageTemplate: messageTemplate);

    public static void Information<T>(string messageTemplate, T propertyValue)
        => Logger.Information(messageTemplate, propertyValue);

    public static void Information<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        => Logger.Information(messageTemplate, propertyValue0, propertyValue1);

    public static void Information<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        => Logger.Information(messageTemplate, propertyValue0, propertyValue1, propertyValue2);

    public static void Information(string messageTemplate, params object[] propertyValues)
        => Logger.Information(messageTemplate, propertyValues: propertyValues);

    public static void Information(Exception exception, string messageTemplate)
        => Logger.Information(exception, messageTemplate);

    public static void Information<T>(Exception exception, string messageTemplate, T propertyValue)
        => Logger.Information(exception, messageTemplate, propertyValue);

    public static void Information<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        => Logger.Information(exception, messageTemplate, propertyValue0, propertyValue1);

    public static void Information<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        => Logger.Information(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);

    public static void Information(Exception exception, string messageTemplate, params object[] propertyValues)
        => Logger.Information(exception, messageTemplate, propertyValues: propertyValues);

    public static void Warning(string messageTemplate)
        => Logger.Warning(messageTemplate: messageTemplate);

    public static void Warning<T>(string messageTemplate, T propertyValue)
        => Logger.Warning(messageTemplate, propertyValue);

    public static void Warning<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        => Logger.Warning(messageTemplate, propertyValue0, propertyValue1);

    public static void Warning<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        => Logger.Warning(messageTemplate, propertyValue0, propertyValue1, propertyValue2);

    public static void Warning(string messageTemplate, params object[] propertyValues)
        => Logger.Warning(messageTemplate, propertyValues: propertyValues);

    public static void Warning(Exception exception, string messageTemplate)
        => Logger.Warning(exception, messageTemplate);

    public static void Warning<T>(Exception exception, string messageTemplate, T propertyValue)
        => Logger.Warning(exception, messageTemplate, propertyValue);

    public static void Warning<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        => Logger.Warning(exception, messageTemplate, propertyValue0, propertyValue1);

    public static void Warning<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        => Logger.Warning(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);

    public static void Warning(Exception exception, string messageTemplate, params object[] propertyValues)
        => Logger.Warning(exception, messageTemplate, propertyValues: propertyValues);

    public static void Error(string messageTemplate)
        => Logger.Error(messageTemplate: messageTemplate);

    public static void Error<T>(string messageTemplate, T propertyValue)
        => Logger.Error(messageTemplate, propertyValue);

    public static void Error<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        => Logger.Error(messageTemplate, propertyValue0, propertyValue1);

    public static void Error<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        => Logger.Error(messageTemplate, propertyValue0, propertyValue1, propertyValue2);

    public static void Error(string messageTemplate, params object[] propertyValues)
        => Logger.Error(messageTemplate, propertyValues: propertyValues);

    public static void Error(Exception exception, string messageTemplate)
        => Logger.Error(exception, messageTemplate);

    public static void Error<T>(Exception exception, string messageTemplate, T propertyValue)
        => Logger.Error(exception, messageTemplate, propertyValue);

    public static void Error<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        => Logger.Error(exception, messageTemplate, propertyValue0, propertyValue1);

    public static void Error<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        => Logger.Error(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);

    public static void Error(Exception exception, string messageTemplate, params object[] propertyValues)
        => Logger.Error(exception, messageTemplate, propertyValues: propertyValues);

    public static void Fatal(string messageTemplate)
        => Logger.Fatal(messageTemplate: messageTemplate);

    public static void Fatal<T>(string messageTemplate, T propertyValue)
        => Logger.Fatal(messageTemplate, propertyValue);

    public static void Fatal<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        => Logger.Fatal(messageTemplate, propertyValue0, propertyValue1);

    public static void Fatal<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        => Logger.Fatal(messageTemplate, propertyValue0, propertyValue1, propertyValue2);

    public static void Fatal(string messageTemplate, params object[] propertyValues)
        => Logger.Fatal(messageTemplate, propertyValues: propertyValues);

    public static void Fatal(Exception exception, string messageTemplate)
        => Logger.Fatal(exception, messageTemplate);

    public static void Fatal<T>(Exception exception, string messageTemplate, T propertyValue)
        => Logger.Fatal(exception, messageTemplate, propertyValue);

    public static void Fatal<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        => Logger.Fatal(exception, messageTemplate, propertyValue0, propertyValue1);

    public static void Fatal<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        => Logger.Fatal(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);

    public static void Fatal(Exception exception, string messageTemplate, params object[] propertyValues)
        => Logger.Fatal(exception, messageTemplate, propertyValues: propertyValues);

    internal static void SetApplication(ApplicationFramework application)
    {
        if (app is not null)
        {
            throw new InvalidOperationException("Application is already set.");
        }

        app = application;
    }
}

using JSSoft.Commands;
using LibplanetConsole.Clients.Executable;
using LibplanetConsole.Frameworks;

try
{
    var settings = ApplicationSettingsParser.Parse<ApplicationSettings>(args);
    var @out = Console.Out;
    await using var application = new Application(settings);
    await @out.WriteLineAsync();
    await application.RunAsync();
    await @out.WriteLineAsync("\u001b0");
}
catch (CommandParsingException e)
{
    CommandUsageUtility.Print(Console.Out, e);
    Environment.Exit(1);
}

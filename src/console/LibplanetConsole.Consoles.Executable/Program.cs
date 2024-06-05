using JSSoft.Commands;
using LibplanetConsole.Consoles.Executable;
using LibplanetConsole.Frameworks;

try
{
    var settings = ApplicationSettingsParser.Parse<ApplicationSettings>(args);
    var @out = Console.Out;
    await using var application = new Application(settings);
    @out.WriteLine();
    await application.RunAsync();
    @out.WriteLine("\u001b0");
}
catch (CommandParsingException e)
{
    CommandUsageUtility.Print(Console.Out, e);
    Environment.Exit(1);
}

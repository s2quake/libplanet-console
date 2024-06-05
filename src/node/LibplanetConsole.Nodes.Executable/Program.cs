using JSSoft.Commands;
using LibplanetConsole.Frameworks;
using LibplanetConsole.Nodes.Executable;

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

using JSSoft.Commands;

namespace OnBoarding.ConsoleHost;

sealed class ApplicationOptions
{
    public const int DefaultUserCount = 10;

    [CommandProperty(InitValue = 1)]
    public int SwarmCount { get; set; }

    [CommandProperty(InitValue = DefaultUserCount)]
    public int UserCount { get; set; }

    [CommandProperty]
    public string StorePath { get; set; } = string.Empty;

    public static ApplicationOptions Parse(string[] args)
    {
        var options = new ApplicationOptions();
        var parserSettings = new CommandSettings()
        {
            AllowEmpty = true,
        };
        var parser = new CommandParser(options, parserSettings);
        parser.Parse(args);
        return options;
    }
}

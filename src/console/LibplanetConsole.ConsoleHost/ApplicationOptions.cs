using JSSoft.Commands;

namespace LibplanetConsole.ConsoleHost;

internal sealed class ApplicationOptions
{
    [CommandProperty]
    public string EndPoint { get; set; } = string.Empty;

    [CommandProperty(InitValue = 4)]
    public int NodeCount { get; set; }

    [CommandProperty(InitValue = 2)]
    public int ClientCount { get; set; }

    [CommandProperty]
    public string GenesisKey { get; set; } = string.Empty;

    public static ApplicationOptions Parse(string[] args)
    {
        var options = new ApplicationOptions();
        var parserSettings = new CommandSettings()
        {
            AllowEmpty = true,
        };
        var parser = new CommandParser(options, parserSettings);
        parser.Parse(args);
        if (options.NodeCount < 1)
        {
            throw new InvalidOperationException("Node count must be greater than or equal to 1.");
        }

        return options;
    }
}

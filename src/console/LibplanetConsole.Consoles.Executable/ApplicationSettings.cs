using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Common.DataAnnotations;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.Consoles.Executable;

[ApplicationSettings]
internal sealed record class ApplicationSettings
{
    [CommandPropertyRequired]
    public string Path { get; set; } = string.Empty;

    [CommandProperty]
    [CommandSummary("The endpoint of the console to run." +
                    "If omitted, one of the random ports will be used.")]
    [AppEndPoint]
    public string EndPoint { get; init; } = string.Empty;

    [CommandPropertySwitch]
    [CommandSummary("If set, the node and the client processes will not run.")]
    public bool NoProcess { get; set; }

    [CommandPropertySwitch]
    [CommandSummary($"If set, the node and the client processes start in a new window.\n" +
                    $"This option cannot be used with --no-process option.")]
    [CommandPropertyCondition(nameof(NoProcess), false)]
    public bool NewWindow { get; set; }

    [CommandPropertySwitch]
    [CommandSummary("If set, the node and the client processes are detached from the console.\n" +
                    "This option cannot be used with --no-process option.\n" +
                    "And this option is only available if the --new-window option is set.")]
    [CommandPropertyCondition(nameof(NewWindow), true)]
    public bool Detach { get; set; }

    [CommandPropertySwitch('m', useName: true)]
    [CommandSummary("If set, The service does not start automatically " +
                    "when the node and client processes are executed.\n" +
                    $"This option cannot be used with --no-process or --detach option.")]
    [CommandPropertyCondition(nameof(NoProcess), false)]
    [CommandPropertyCondition(nameof(Detach), false)]
    public bool ManualStart { get; set; }

    public ApplicationOptions ToOptions(object[] components)
    {
        var path = Path;
        var endPoint = AppEndPoint.ParseOrNext(EndPoint);
        return new ApplicationOptions(path, endPoint)
        {
            NoProcess = NoProcess,
            NewWindow = NewWindow,
            Detach = Detach,
            ManualStart = ManualStart,
            Components = components,
        };
    }
}

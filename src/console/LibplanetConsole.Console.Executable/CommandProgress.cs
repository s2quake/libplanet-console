using JSSoft.Commands;
using ShellProgressBar;

namespace LibplanetConsole.Console.Executable;

public sealed class CommandProgress : IProgress<ProgressInfo>, IDisposable
{
    private const int TotalTicks = 10000;
    private readonly ProgressBar _progressBar;
    private readonly IProgress<ProgressInfo> _progress;
    private bool _isDisposed;

    public CommandProgress()
    {
        var options = new ProgressBarOptions
        {
            ForegroundColor = ConsoleColor.Yellow,
            ForegroundColorDone = ConsoleColor.DarkGreen,
            BackgroundColor = ConsoleColor.DarkGray,
            BackgroundCharacter = '▉',
        };
        _progressBar = new ProgressBar(TotalTicks, "progress bar is on the bottom now", options);
        _progress = _progressBar.AsProgress<ProgressInfo>(
            message: (pi) => pi.Text,
            percentage: (pi) => pi.Value);
    }

    public void Dispose()
    {
        if (_isDisposed is false)
        {
            _progressBar.Dispose();
            _isDisposed = true;
        }
    }

    public void Report(ProgressInfo value) => _progress.Report(value);
}

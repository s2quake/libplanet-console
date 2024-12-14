using System.Diagnostics;
using JSSoft.Commands;

namespace LibplanetConsole.Node.Explorer.Commands;

[CommandSummary("Opens the explorer")]
internal sealed class ExplorerCommand(IExplorer explorer)
    : CommandBase
{
    protected override void OnExecute()
    {
        var url = $"{explorer.Url}";
        if (OperatingSystem.IsWindows())
        {
            url = url.Replace("&", "^&");
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
        else if (OperatingSystem.IsLinux())
        {
            Process.Start("xdg-open", url);
        }
        else if (OperatingSystem.IsMacOS())
        {
            Process.Start("open", url);
        }
        else
        {
            throw new NotSupportedException("The operating system is not supported.");
        }
    }
}

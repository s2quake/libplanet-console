using JSSoft.Commands;
using Microsoft.Extensions.Hosting;

namespace LibplanetConsole.Client.Commands;

[CommandSummary("Exits the application")]
internal sealed class ExitCommand(IHostApplicationLifetime applicationLifetime)
    : CommandAsyncBase
{
    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        var resetEvent = new ManualResetEvent(false);
        applicationLifetime.ApplicationStopping.Register(() =>
        {
            resetEvent.Set();
        });
        applicationLifetime.StopApplication();
        while (!resetEvent.WaitOne(1))
        {
            await Task.Delay(1, default);
        }
    }
}

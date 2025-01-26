using LibplanetConsole.Client.Services;
using LibplanetConsole.Grpc.Console;
using Microsoft.Extensions.Options;

namespace LibplanetConsole.Client.Executable;

internal sealed class ConsoleConfigureOptions(EndPoint consoleEndPoint)
    : IConfigureOptions<ApplicationOptions>
{
    public void Configure(ApplicationOptions options)
    {
        using var channel = ConsoleChannel.CreateChannel(consoleEndPoint);
        var service = new ConsoleService(channel);
        var request = new GetClientSettingsRequest();
        var response = service.GetClientSettings(
            request: request,
            deadline: DateTime.UtcNow.AddSeconds(5));

        options.ParentProcessId = response.ProcessId;
        options.NodeEndPoint = response.NodeEndPoint;
    }
}

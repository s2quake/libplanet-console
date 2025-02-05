using LibplanetConsole.Common;
using LibplanetConsole.Console.Grpc;
using LibplanetConsole.Console.Services;
using Microsoft.Extensions.Options;

namespace LibplanetConsole.Client.Executable;

internal sealed class ConsoleConfigureOptions
    : IConfigureOptions<ApplicationOptions>
{
    public Uri? ConsoleUrl { get; private set; }

    public void Configure(ApplicationOptions options)
    {
        var hubUrl = UriUtility.ParseOrDefault(options.HubUrl);
        if (options.ParentProcessId is 0 && hubUrl is not null)
        {
            using var channel = ConsoleChannel.CreateChannel(hubUrl);
            var service = new ConsoleService(channel);
            var request = new GetClientSettingsRequest();
            var response = service.GetClientSettings(
                request: request);

            options.ParentProcessId = response.ProcessId;
            ConsoleUrl = hubUrl;
        }
    }
}

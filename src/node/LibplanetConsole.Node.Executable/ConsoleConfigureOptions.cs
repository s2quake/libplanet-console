using Google.Protobuf;
using LibplanetConsole.Common;
using LibplanetConsole.Console.Grpc;
using LibplanetConsole.Console.Services;
using LibplanetConsole.Grpc;
using Microsoft.Extensions.Options;

namespace LibplanetConsole.Node.Executable;

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
            var request = new GetNodeSettingsRequest();
            var response = service.GetNodeSettings(
                request: request);

            options.Genesis = GetGenesis(response.Genesis);
            options.AppProtocolVersion = response.AppProtocolVersion;
            options.ParentProcessId = response.ProcessId;
            ConsoleUrl = hubUrl;
        }
    }

    private static string GetGenesis(ByteString genesisByteString)
    {
        var bytes = TypeUtility.ToByteArray(genesisByteString);
        var genesis = BlockUtility.DeserializeBlock(bytes);
        return BlockUtility.ToString(genesis);
    }
}

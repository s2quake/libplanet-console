using Google.Protobuf;
using LibplanetConsole.Console.Grpc;
using LibplanetConsole.Console.Services;
using LibplanetConsole.Grpc;
using Microsoft.Extensions.Options;

namespace LibplanetConsole.Node.Executable;

internal sealed class ConsoleConfigureOptions(EndPoint consoleEndPoint)
    : IConfigureOptions<ApplicationOptions>
{
    public void Configure(ApplicationOptions options)
    {
        using var channel = ConsoleChannel.CreateChannel(consoleEndPoint);
        var service = new ConsoleService(channel);
        var request = new GetNodeSettingsRequest();
        var response = service.GetNodeSettings(
            request: request,
            deadline: DateTime.UtcNow.AddSeconds(5));

        options.Genesis = GetGenesis(response.Genesis);
        options.AppProtocolVersion = response.AppProtocolVersion;
        options.ParentProcessId = response.ProcessId;
        options.SeedEndPoint = response.SeedEndPoint;
    }

    private static string GetGenesis(ByteString genesisByteString)
    {
        var bytes = TypeUtility.ToByteArray(genesisByteString);
        var genesis = BlockUtility.DeserializeBlock(bytes);
        return BlockUtility.ToString(genesis);
    }
}

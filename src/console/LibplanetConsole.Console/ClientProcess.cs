using LibplanetConsole.Common;
using static LibplanetConsole.Common.EndPointUtility;

namespace LibplanetConsole.Console;

internal sealed class ClientProcess(ClientOptions clientOptions)
    : ClientProcessBase
{
    public bool Detach { get; set; }

    public IList<string> ExtendedArguments { get; set; } = [];

    public override string[] Arguments
    {
        get
        {
            var argumentList = new List<string>();
            if (clientOptions.RepositoryPath != string.Empty)
            {
                argumentList.Add("start");
                argumentList.Add(clientOptions.RepositoryPath);
            }
            else
            {
                if (GetHost(clientOptions.EndPoint) is not "localhost")
                {
                    throw new InvalidOperationException("EndPoint must be localhost.");
                }

                argumentList.Add("run");
                argumentList.Add("--port");
                argumentList.Add($"{GetPort(clientOptions.EndPoint)}");
                argumentList.Add("--private-key");
                argumentList.Add(PrivateKeyUtility.ToString(clientOptions.PrivateKey));

                if (clientOptions.LogPath != string.Empty)
                {
                    argumentList.Add("--log-path");
                    argumentList.Add(clientOptions.LogPath);
                }

                if (clientOptions.NodeEndPoint is { } nodeEndPoint)
                {
                    argumentList.Add("--node-end-point");
                    argumentList.Add(EndPointUtility.ToString(nodeEndPoint));
                }
            }

            if (NewWindow != true)
            {
                argumentList.Add("--no-repl");
            }

            if (Detach != true)
            {
                argumentList.Add("--parent");
                argumentList.Add(Environment.ProcessId.ToString());
            }

            if (ExtendedArguments.Count > 0)
            {
                argumentList.AddRange(ExtendedArguments);
            }

            return [.. argumentList];
        }
    }
}

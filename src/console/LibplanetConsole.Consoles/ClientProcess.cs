using LibplanetConsole.Common;
using static LibplanetConsole.Consoles.ProcessEnvironment;

namespace LibplanetConsole.Consoles;

internal sealed class ClientProcess(Client client, ClientOptions clientOptions)
    : ClientProcessBase
{
    public bool Detach { get; set; }

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
                argumentList.Add("run");
                argumentList.Add("--end-point");
                argumentList.Add(clientOptions.EndPoint.ToString());
                argumentList.Add("--private-key");
                argumentList.Add(AppPrivateKey.ToString(clientOptions.PrivateKey));

                if (clientOptions.LogPath != string.Empty)
                {
                    argumentList.Add("--log-path");
                    argumentList.Add(clientOptions.LogPath);
                }

                if (clientOptions.NodeEndPoint is { } nodeEndPoint)
                {
                    argumentList.Add("--node-end-point");
                    argumentList.Add(nodeEndPoint.ToString());
                }

                var extendedArguments = GetArguments(serviceProvider: client, obj: client);
                argumentList.AddRange(extendedArguments);
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

            return [.. argumentList];
        }
    }
}

using LibplanetConsole.Common;

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
                var url = clientOptions.Url;
                if (url.Host is not "localhost")
                {
                    throw new InvalidOperationException("Url must be localhost.");
                }

                argumentList.Add("run");
                argumentList.Add("--port");
                argumentList.Add($"{url.Port}");
                argumentList.Add("--private-key");
                argumentList.Add(PrivateKeyUtility.ToString(clientOptions.PrivateKey));

                if (clientOptions.LogPath != string.Empty)
                {
                    argumentList.Add("--log-path");
                    argumentList.Add(clientOptions.LogPath);
                }

                if (clientOptions.HubUrl is { } nodeUrl)
                {
                    argumentList.Add("--node-url");
                    argumentList.Add(nodeUrl.ToString());
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

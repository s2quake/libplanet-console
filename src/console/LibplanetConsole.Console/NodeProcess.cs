using LibplanetConsole.Common;
using static LibplanetConsole.Console.ProcessEnvironment;

namespace LibplanetConsole.Console;

internal sealed class NodeProcess(Node node, NodeOptions nodeOptions) : NodeProcessBase
{
    public bool Detach { get; set; }

    public override string[] Arguments
    {
        get
        {
            var argumentList = new List<string>();
            if (nodeOptions.RepositoryPath != string.Empty)
            {
                argumentList.Add("start");
                argumentList.Add(nodeOptions.RepositoryPath);
            }
            else
            {
                argumentList.Add("run");
                argumentList.Add("--end-point");
                argumentList.Add(EndPointUtility.ToString(nodeOptions.EndPoint));
                argumentList.Add("--private-key");
                argumentList.Add(PrivateKeyUtility.ToString(nodeOptions.PrivateKey));

                if (nodeOptions.StorePath != string.Empty)
                {
                    argumentList.Add("--store-path");
                    argumentList.Add(nodeOptions.StorePath);
                }

                if (nodeOptions.LogPath != string.Empty)
                {
                    argumentList.Add("--log-path");
                    argumentList.Add(nodeOptions.LogPath);
                }

                if (nodeOptions.SeedEndPoint is { } seedEndPoint)
                {
                    argumentList.Add("--seed-end-point");
                    argumentList.Add(EndPointUtility.ToString(seedEndPoint));
                }

                var extendedArguments = GetArguments(serviceProvider: node, obj: node);
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

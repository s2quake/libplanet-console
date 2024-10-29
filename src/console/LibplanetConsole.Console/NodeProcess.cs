using LibplanetConsole.Common;
using static LibplanetConsole.Common.EndPointUtility;
using static LibplanetConsole.Console.ProcessEnvironment;

namespace LibplanetConsole.Console;

internal sealed class NodeProcess(Node node, NodeOptions nodeOptions) : NodeProcessBase
{
    public bool Detach { get; set; }

    public IList<string> ExtendedArguments { get; set; } = [];

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
                if (GetHost(nodeOptions.EndPoint) is not "localhost")
                {
                    throw new InvalidOperationException("EndPoint must be localhost.");
                }

                argumentList.Add("run");
                argumentList.Add("--port");
                argumentList.Add($"{GetPort(nodeOptions.EndPoint)}");
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

                if (nodeOptions.ActionProviderModulePath != string.Empty)
                {
                    argumentList.Add("--module-path");
                    argumentList.Add(nodeOptions.ActionProviderModulePath);
                }

                if (nodeOptions.ActionProviderType != string.Empty)
                {
                    argumentList.Add("--module-type");
                    argumentList.Add(nodeOptions.ActionProviderType);
                }

                var extendedArguments = GetArguments(serviceProvider: node, obj: node);
                argumentList.AddRange(extendedArguments);
            }

            if (NewWindow is false)
            {
                argumentList.Add("--no-repl");
            }

            if (Detach is false)
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

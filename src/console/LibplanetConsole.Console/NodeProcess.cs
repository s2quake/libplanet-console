using LibplanetConsole.Common;

namespace LibplanetConsole.Console;

internal sealed class NodeProcess(NodeOptions nodeOptions) : NodeProcessBase
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
                var nodeUrl = nodeOptions.Url;
                if (nodeUrl.Host is not "localhost")
                {
                    throw new InvalidOperationException("Url must be localhost.");
                }

                argumentList.Add("run");
                argumentList.Add("--port");
                argumentList.Add($"{nodeUrl.Port}");
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

                if (nodeOptions.HubUrl is { } hubUrl)
                {
                    argumentList.Add("--hub-url");
                    argumentList.Add(hubUrl.ToString());
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

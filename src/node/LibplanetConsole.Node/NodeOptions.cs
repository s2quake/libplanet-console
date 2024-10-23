using LibplanetConsole.Options;

namespace LibplanetConsole.Node;

[Options]
public sealed class NodeOptions : OptionsBase<NodeOptions>
{
    public const string Position = "Node";

    public int BlocksyncPort { get; set; }

    public int ConsensusPort { get; set; }
}

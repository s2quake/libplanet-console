namespace LibplanetConsole.Nodes.Services;

public interface INodeCallback
{
    void OnStarted(NodeInfo nodeInfo);

    void OnStopped();
}

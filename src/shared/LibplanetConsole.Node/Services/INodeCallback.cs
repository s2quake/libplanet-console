namespace LibplanetConsole.Node.Services;

public interface INodeCallback
{
    void OnStarted(NodeInfo nodeInfo);

    void OnStopped();
}

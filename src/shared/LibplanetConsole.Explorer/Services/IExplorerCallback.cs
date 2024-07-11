namespace LibplanetConsole.Explorer.Services;

public interface IExplorerCallback
{
    void OnStarted(ExplorerInfo explorerInfo);

    void OnStopped();
}

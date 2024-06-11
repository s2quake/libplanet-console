namespace LibplanetConsole.Examples.Services;

public interface IExampleNodeCallback
{
    void OnSubscribed(string address);

    void OnUnsubscribed(string address);
}

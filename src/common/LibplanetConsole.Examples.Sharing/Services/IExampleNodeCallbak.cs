namespace LibplanetConsole.Examples;

public interface IExampleNodeCallbak
{
    void OnSubscribed(string address);

    void OnUnsubscribed(string address);
}

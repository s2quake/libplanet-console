namespace LibplanetConsole.Examples.Services;

public interface IExampleNodeCallbak
{
    void OnSubscribed(string address);

    void OnUnsubscribed(string address);
}

namespace LibplanetConsole.Example.Services;

public interface IExampleNodeCallback
{
    void OnSubscribed(Address address);

    void OnUnsubscribed(Address address);
}

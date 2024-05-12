namespace LibplanetConsole.Common.QuickStarts;

public interface ISampleNodeCallbak
{
    void OnSubscribed(string address);

    void OnUnsubscribed(string address);
}

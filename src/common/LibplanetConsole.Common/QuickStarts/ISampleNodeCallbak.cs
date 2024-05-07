using JSSoft.Communication;

namespace LibplanetConsole.Common.QuickStarts;

public interface ISampleNodeCallbak
{
    [ClientMethod]
    void OnSubscribed(string address);

    [ClientMethod]
    void OnUnsubscribed(string address);
}

using JSSoft.Communication;

namespace LibplanetConsole.Common.QuickStarts;

public interface ISampleClientService
{
    [ServerMethod(IsOneWay = true)]
    void Subscribe();

    [ServerMethod(IsOneWay = true)]
    void Unsubscribe();
}

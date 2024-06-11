namespace LibplanetConsole.Common;

public interface IInfoProvider
{
    bool CanSupport(Type type);

    IEnumerable<(string Name, object? Value)> GetInfos(object obj);
}

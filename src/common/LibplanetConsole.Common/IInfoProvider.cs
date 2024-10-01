namespace LibplanetConsole.Common;

public interface IInfoProvider
{
    string Name { get; }

    bool CanSupport(Type type);

    object? GetInfo(object obj);
}

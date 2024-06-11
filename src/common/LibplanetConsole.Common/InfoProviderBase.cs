namespace LibplanetConsole.Common;

public abstract class InfoProviderBase<T> : IInfoProvider
{
    bool IInfoProvider.CanSupport(Type type) => typeof(T).IsAssignableFrom(type) == true;

    IEnumerable<(string Name, object? Value)> IInfoProvider.GetInfos(object obj)
    {
        if (obj is T t)
        {
            return GetInfos(t);
        }

        throw new NotSupportedException("The object is not supported.");
    }

    protected abstract IEnumerable<(string Name, object? Value)> GetInfos(T obj);
}

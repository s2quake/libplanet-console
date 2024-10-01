namespace LibplanetConsole.Common;

public abstract class InfoProviderBase<T>(string name) : IInfoProvider
{
    public string Name { get; } = name;

    bool IInfoProvider.CanSupport(Type type) => typeof(T).IsAssignableFrom(type) == true;

    object? IInfoProvider.GetInfo(object obj)
    {
        if (obj is T t)
        {
            return GetInfo(t);
        }

        throw new NotSupportedException("The object is not supported.");
    }

    protected abstract object? GetInfo(T obj);
}

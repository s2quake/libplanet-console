namespace LibplanetConsole.Consoles;

public abstract class ProcessArgumentProviderBase<T> : IProcessArgumentProvider
{
    bool IProcessArgumentProvider.CanSupport(Type type) => typeof(T).IsAssignableFrom(type) == true;

    IEnumerable<string> IProcessArgumentProvider.GetArguments(object obj)
    {
        if (obj is T t)
        {
            return GetArguments(t);
        }

        throw new NotSupportedException("The object is not supported.");
    }

    protected abstract IEnumerable<string> GetArguments(T obj);
}

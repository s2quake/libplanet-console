namespace LibplanetConsole.Common.Threading;

public sealed class CriticalSection(string name)
{
    private int _i;

    public IDisposable Scope()
    {
        if (Interlocked.CompareExchange(ref _i, 1, 0) == 0)
        {
            return new ScopeObject(() => Interlocked.Exchange(ref _i, 0));
        }

        throw new InvalidOperationException($"'{name}' is running.");
    }

    private sealed class ScopeObject(Action action) : IDisposable
    {
        public void Dispose() => action.Invoke();
    }
}

namespace LibplanetConsole.Console;

public sealed class ExecutionScope
{
    private int _reference;

    public bool IsExecuting => _reference > 0;

    public IDisposable Enter()
    {
        return new ScopeObject(this);
    }

    private sealed class ScopeObject : IDisposable
    {
        private readonly ExecutionScope _scope;

        public ScopeObject(ExecutionScope scope)
        {
            _scope = scope;
            Interlocked.Increment(ref _scope._reference);
        }

        public void Dispose()
        {
            Interlocked.Decrement(ref _scope._reference);
            GC.SuppressFinalize(this);
        }
    }
}

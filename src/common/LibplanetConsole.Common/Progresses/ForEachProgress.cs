namespace LibplanetConsole.Common.Progresses;

public sealed class ForEachProgress<T> : Progress<T>
{
    private int _ticketed;
    private int _completed;

    public ForEachProgress(int length)
    {
        if (length <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(length), length, "The length must be greater than 0.");
        }

        Length = length;
    }

    public int Length { get; }

    public int Completion => _completed;

    public IDisposable Tick(T value)
    {
        if (_ticketed >= Length)
        {
            throw new InvalidOperationException("The limit than can be ticked has been exceeded.");
        }

        _ticketed++;
        OnReport(value);
        return new Item(this, value);
    }

    private void Complete(T value)
    {
        if (_completed >= Length)
        {
            throw new InvalidOperationException("The progress has already been completed.");
        }

        _completed++;
        if (_completed == Length)
        {
            OnReport(value);
        }
    }

    private sealed class Item(ForEachProgress<T> progress, T value) : IDisposable
    {
        private bool _isDisposed;

        public void Dispose()
        {
            if (_isDisposed is false)
            {
                progress.Complete(value);
                _isDisposed = true;
            }
        }
    }
}

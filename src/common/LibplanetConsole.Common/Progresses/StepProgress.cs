namespace LibplanetConsole.Common.Progresses;

public sealed class StepProgress<T> : Progress<T>
{
    private readonly HashSet<T> _items;
    private bool _isCompleted;

    public StepProgress(int length)
    {
        if (length <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(length), length, "The length must be greater than 0.");
        }

        _items = new HashSet<T>(length + 1);
        Length = length;
    }

    public int Length { get; }

    public int Step => _isCompleted is true ? Length : _items.Count - 1;

    public void Next(T value)
    {
        if (Step >= Length)
        {
            throw new InvalidOperationException("The progress has already been completed.");
        }

        _items.Add(value);
        OnReport(value);
    }

    public void Complete(T value)
    {
        if (_isCompleted is true)
        {
            throw new InvalidOperationException("The progress has already been completed.");
        }

        _isCompleted = true;
        OnReport(value);
    }
}

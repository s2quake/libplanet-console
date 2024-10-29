#pragma warning disable SA1402 // File may only contain a single type
using System.Diagnostics;

namespace LibplanetConsole.Grpc;

internal sealed class EventItem<TResponse> : IEventItem<TResponse>
{
    public required Action<EventHandler> Attach { get; init; }

    public required Action<EventHandler> Detach { get; init; }

    public required Func<TResponse> Selector { get; init; }

    public TResponse GetResponse(EventArgs e)
    {
        return Selector();
    }

    void IEventItem<TResponse>.Attach(EventHandler handler)
    {
        Attach(handler);
    }

    void IEventItem<TResponse>.Detach(EventHandler handler)
    {
        Detach(handler);
    }
}

internal sealed class EventItem<TResponse, TEventArgs> : IEventItem<TResponse>
    where TEventArgs : EventArgs
{
    private EventHandler? _handler;

    public required Action<EventHandler<TEventArgs>> Attach { get; init; }

    public required Action<EventHandler<TEventArgs>> Detach { get; init; }

    public required Func<TEventArgs, TResponse> Selector { get; init; }

    public TResponse GetResponse(EventArgs e)
    {
        if (e is TEventArgs args)
        {
            return Selector(args);
        }

        throw new UnreachableException("Unexpected event argument type.");
    }

    void IEventItem<TResponse>.Attach(EventHandler handler)
    {
        Attach(Invoke);
        _handler = handler;
    }

    void IEventItem<TResponse>.Detach(EventHandler handler)
    {
        _handler = null;
        Detach(Invoke);
    }

    private void Invoke(object? sender, TEventArgs e)
    {
        _handler?.Invoke(sender, e);
    }
}

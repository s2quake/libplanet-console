#pragma warning disable SA1402 // File may only contain a single type
using Grpc.Core;

namespace LibplanetConsole.Grpc;

internal interface IEventItem<TResponse>
{
    void Attach(EventHandler handler);

    void Detach(EventHandler handler);

    TResponse GetResponse(EventArgs e);
}

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
    public required Action<EventHandler<TEventArgs>> Attach { get; init; }

    public required Action<EventHandler<TEventArgs>> Detach { get; init; }

    public required Func<TEventArgs, TResponse> Selector { get; init; }

    public TResponse GetResponse(EventArgs e)
    {
        if (e is TEventArgs args)
        {
            return Selector(args);
        }

        throw new ArgumentException();
    }

    private EventHandler? _handler;

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

    private void Invoke(object sender, TEventArgs e)
    {
        _handler?.Invoke(sender, e);
    }
}

internal sealed class EventStreamer<TResponse>(IAsyncStreamWriter<TResponse> streamWriter)
    : Streamer<TResponse>(streamWriter)
{
    public IList<IEventItem<TResponse>> Items { get; } = [];

    protected async override Task OnRun(CancellationToken cancellationToken)
    {
        var handlers = new EventHandler[Items.Count];
        for (var i = 0; i < Items.Count; i++)
        {
            var item = Items[i];

            var handler = new EventHandler(Handler);
            item.Attach(Handler);
            handlers[i] = handler;

            async void Handler(object? s, EventArgs args)
            {
                var value = item.GetResponse(args);
                await WriteValueAsync(value);
            }
        }

        try
        {
            await base.OnRun(cancellationToken);
        }
        finally
        {
            for (var i = 0; i < Items.Count; i++)
            {
                var item = Items[i];
                var handler = handlers[i];
                item.Detach(handler);
            }
        }
    }
}

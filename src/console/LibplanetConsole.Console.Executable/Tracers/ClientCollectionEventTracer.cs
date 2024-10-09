using System.Collections.Specialized;
using JSSoft.Terminals;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Console.Executable.Tracers;

internal sealed class ClientCollectionEventTracer : IHostedService, IDisposable
{
    private readonly IClientCollection _clients;

    public ClientCollectionEventTracer(IClientCollection clients)
    {
        _clients = clients;
        foreach (var client in _clients)
        {
            AttachEvent(client);
        }

        _clients.CollectionChanged += Clients_CollectionChanged;
    }

    public Task StartAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;

    void IDisposable.Dispose()
    {
        foreach (var client in _clients)
        {
            DetachEvent(client);
        }

        _clients.CollectionChanged -= Clients_CollectionChanged;
    }

    private void AttachEvent(IClient client)
    {
        client.Attached += Client_Attached;
        client.Detached += Client_Detached;
        client.Started += Client_Started;
        client.Stopped += Client_Stopped;
    }

    private void DetachEvent(IClient client)
    {
        client.Attached -= Client_Attached;
        client.Detached -= Client_Detached;
        client.Started -= Client_Started;
        client.Stopped -= Client_Stopped;
    }

    private void Clients_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            foreach (IClient client in e.NewItems!)
            {
                var message = $"Client created: {client.Address.ToShortString()}";
                var colorType = TerminalColorType.BrightBlue;
                System.Console.Out.WriteColoredLine(message, colorType);
                AttachEvent(client);
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            foreach (IClient client in e.OldItems!)
            {
                var message = $"Client deleted: {client.Address.ToShortString()}";
                var colorType = TerminalColorType.BrightBlue;
                System.Console.Out.WriteColoredLine(message, colorType);
                DetachEvent(client);
            }
        }
    }

    private void Client_Attached(object? sender, EventArgs e)
    {
        if (sender is IClient client)
        {
            var message = $"Client attached: {client.Address.ToShortString()}";
            var colorType = TerminalColorType.BrightBlue;
            System.Console.Out.WriteColoredLine(message, colorType);
        }
    }

    private void Client_Detached(object? sender, EventArgs e)
    {
        if (sender is IClient client)
        {
            var message = $"Client detached: {client.Address.ToShortString()}";
            var colorType = TerminalColorType.BrightBlue;
            System.Console.Out.WriteColoredLine(message, colorType);
        }
    }

    private void Client_Started(object? sender, EventArgs e)
    {
        if (sender is IClient client)
        {
            var message = $"Client started: {client.Address.ToShortString()}";
            var colorType = TerminalColorType.BrightBlue;
            System.Console.Out.WriteColoredLine(message, colorType);
        }
    }

    private void Client_Stopped(object? sender, EventArgs e)
    {
        if (sender is IClient client)
        {
            var message = $"Client stopped: {client.Address.ToShortString()}";
            var colorType = TerminalColorType.BrightBlue;
            System.Console.Out.WriteColoredLine(message, colorType);
        }
    }
}

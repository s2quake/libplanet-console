using System.Collections.Specialized;
using System.ComponentModel.Composition;
using JSSoft.Terminals;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.Consoles.Executable.Tracers;

[Export(typeof(IApplicationService))]
[method: ImportingConstructor]
internal sealed class ClientCollectionEventTracer(IClientCollection clients) : IApplicationService
{
    private readonly IClientCollection _clients = clients;

    public Task InitializeAsync(
        IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        foreach (var client in _clients)
        {
            AttachEvent(client);
        }

        _clients.CollectionChanged += Clients_CollectionChanged;
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        foreach (var client in _clients)
        {
            DetachEvent(client);
        }

        _clients.CollectionChanged -= Clients_CollectionChanged;
        return ValueTask.CompletedTask;
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
                var message = $"Client created: {client.Address:S}";
                var colorType = TerminalColorType.BrightBlue;
                Console.Out.WriteColoredLine(message, colorType);
                AttachEvent(client);
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            foreach (IClient client in e.OldItems!)
            {
                var message = $"Client deleted: {client.Address:S}";
                var colorType = TerminalColorType.BrightBlue;
                Console.Out.WriteColoredLine(message, colorType);
                DetachEvent(client);
            }
        }
    }

    private void Client_Attached(object? sender, EventArgs e)
    {
        if (sender is IClient client)
        {
            var message = $"Client attached: {client.Address:S}";
            var colorType = TerminalColorType.BrightBlue;
            Console.Out.WriteColoredLine(message, colorType);
        }
    }

    private void Client_Detached(object? sender, EventArgs e)
    {
        if (sender is IClient client)
        {
            var message = $"Client detached: {client.Address:S}";
            var colorType = TerminalColorType.BrightBlue;
            Console.Out.WriteColoredLine(message, colorType);
        }
    }

    private void Client_Started(object? sender, EventArgs e)
    {
        if (sender is IClient client)
        {
            var message = $"Client started: {client.Address:S}";
            var colorType = TerminalColorType.BrightBlue;
            Console.Out.WriteColoredLine(message, colorType);
        }
    }

    private void Client_Stopped(object? sender, EventArgs e)
    {
        if (sender is IClient client)
        {
            var message = $"Client stopped: {client.Address:S}";
            var colorType = TerminalColorType.BrightBlue;
            Console.Out.WriteColoredLine(message, colorType);
        }
    }
}

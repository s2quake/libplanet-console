using System.Collections.Specialized;
using System.ComponentModel.Composition;
using JSSoft.Terminals;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Consoles;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.Consoles.Executable;

[Export(typeof(IApplicationService))]
[method: ImportingConstructor]
internal sealed class ClientCollectionEventTracer(IClientCollection clients) : IApplicationService
{
    private readonly IClientCollection _clients = clients;
    private IClient? _current;

    public static TextWriter Writer => Console.Out;

    public Task InitializeAsync(
        IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        UpdateCurrent(_clients.Current);
        foreach (var client in _clients)
        {
            AttachEvent(client);
        }

        _clients.CurrentChanged += Clients_CurrentChanged;
        _clients.CollectionChanged += Clients_CollectionChanged;
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        UpdateCurrent(null);
        foreach (var client in _clients)
        {
            DetachEvent(client);
        }

        _clients.CurrentChanged -= Clients_CurrentChanged;
        _clients.CollectionChanged -= Clients_CollectionChanged;
        return ValueTask.CompletedTask;
    }

    private void UpdateCurrent(IClient? client)
    {
        if (_current is not null)
        {
        }

        _current = client;

        if (_current is not null)
        {
        }
    }

    private void AttachEvent(IClient client)
    {
        client.Started += Client_Started;
        client.Stopped += Client_Stopped;
    }

    private void DetachEvent(IClient client)
    {
        client.Started -= Client_Started;
        client.Stopped -= Client_Stopped;
    }

    private void Clients_CurrentChanged(object? sender, EventArgs e)
    {
        UpdateCurrent(_clients.Current);
    }

    private void Clients_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            foreach (IClient client in e.NewItems!)
            {
                var message = $"Client attached: {(ShortAddress)client.Address}";
                var colorType = TerminalColorType.BrightBlue;
                Console.Out.WriteColoredLine(message, colorType);
                AttachEvent(client);
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            foreach (IClient client in e.OldItems!)
            {
                var message = $"Client detached: {(ShortAddress)client.Address}";
                var colorType = TerminalColorType.BrightBlue;
                Console.Out.WriteColoredLine(message, colorType);
                DetachEvent(client);
            }
        }
    }

    private void Client_Started(object? sender, EventArgs e)
    {
        if (sender is IClient client)
        {
            var message = $"Client started: {(ShortAddress)client.Address}";
            var colorType = TerminalColorType.BrightBlue;
            Console.Out.WriteColoredLine(message, colorType);
        }
    }

    private void Client_Stopped(object? sender, EventArgs e)
    {
        if (sender is IClient client)
        {
            var message = $"Client stopped: {(ShortAddress)client.Address}";
            var colorType = TerminalColorType.BrightBlue;
            Console.Out.WriteColoredLine(message, colorType);
        }
    }
}

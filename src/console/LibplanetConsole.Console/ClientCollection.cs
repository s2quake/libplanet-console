using System.Collections;
using System.Collections.Specialized;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Framework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LibplanetConsole.Console;

[Dependency(typeof(NodeCollection))]
internal sealed class ClientCollection(
    IServiceProvider serviceProvider, ClientOptions[] clientOptions)
    : IEnumerable<Client>, IClientCollection, IApplicationService, IAsyncDisposable
{
    private static readonly object LockObject = new();
    private readonly List<Client> _clientList = new(clientOptions.Length);
    private readonly ILogger _logger = serviceProvider.GetLogger<ClientCollection>();
    private Client? _current;
    private bool _isDisposed;

    public event EventHandler? CurrentChanged;

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    public Client? Current
    {
        get => _current;
        set
        {
            if (value is not null && _clientList.Contains(value) == false)
            {
                throw new ArgumentException(
                    message: $"'{value}' is not included in the collection.",
                    paramName: nameof(value));
            }

            _current = value;
            CurrentChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public int Count => _clientList.Count;

    IClient? IClientCollection.Current
    {
        get => Current;
        set
        {
            if (value is not Client client)
            {
                throw new ArgumentException(
                    message: $"'{value}' is not included in the collection.",
                    paramName: nameof(value));
            }

            Current = client;
        }
    }

    public Client this[int index] => _clientList[index];

    public Client this[Address address] => _clientList.Single(item => item.Address == address);

    IClient IClientCollection.this[int index] => this[index];

    IClient IClientCollection.this[Address address] => this[address];

    public bool Contains(Client item) => _clientList.Contains(item);

    public bool Contains(Address address) => _clientList.Exists(item => item.Address == address);

    public int IndexOf(Client item)
    {
        for (var i = 0; i < _clientList.Count; i++)
        {
            if (Equals(item, _clientList[i]) == true)
            {
                return i;
            }
        }

        return -1;
    }

    public int IndexOf(Address address)
    {
        for (var i = 0; i < _clientList.Count; i++)
        {
            if (Equals(address, _clientList[i].Address) == true)
            {
                return i;
            }
        }

        return -1;
    }

    public async Task<Client> AddNewAsync(
        AddNewClientOptions options, CancellationToken cancellationToken)
    {
        var client = ClientFactory.CreateNew(serviceProvider, options.ClientOptions);
        if (options.NoProcess != true)
        {
            await client.StartProcessAsync(options, cancellationToken);
        }

        if (options.NoProcess != true && options.Detach != true)
        {
            await client.AttachAsync(cancellationToken);
        }

        if (client.IsAttached is true && options.ClientOptions.NodeEndPoint is null)
        {
            var nodes = serviceProvider.GetRequiredService<NodeCollection>();
            var node = nodes.RandomNode();
            await client.StartAsync(node, cancellationToken);
        }

        InsertClient(client);
        return client;
    }

    async Task IApplicationService.InitializeAsync(CancellationToken cancellationToken)
    {
        var info = serviceProvider.GetRequiredService<IApplication>().Info;
        await Parallel.ForAsync(0, _clientList.Capacity, cancellationToken, BodyAsync);
        Current = _clientList.FirstOrDefault();

        async ValueTask BodyAsync(int index, CancellationToken cancellationToken)
        {
            var options = new AddNewClientOptions
            {
                ClientOptions = clientOptions[index],
                NoProcess = info.NoProcess,
                Detach = info.Detach,
                NewWindow = info.NewWindow,
            };
            await AddNewAsync(options, cancellationToken);
        }
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        if (_isDisposed is false)
        {
            for (var i = _clientList.Count - 1; i >= 0; i--)
            {
                var client = _clientList[i]!;
                client.Disposed -= Client_Disposed;
                await ClientFactory.DisposeScopeAsync(client);
                _logger.LogDebug("Disposed a client: {Address}", client.Address);
            }

            _isDisposed = true;
            GC.SuppressFinalize(this);
        }
    }

    async Task<IClient> IClientCollection.AddNewAsync(
        AddNewClientOptions options, CancellationToken cancellationToken)
        => await AddNewAsync(options, cancellationToken);

    bool IClientCollection.Contains(IClient item) => item switch
    {
        Client client => Contains(client),
        _ => false,
    };

    int IClientCollection.IndexOf(IClient item) => item switch
    {
        Client client => IndexOf(client),
        _ => -1,
    };

    IEnumerator<Client> IEnumerable<Client>.GetEnumerator() => _clientList.GetEnumerator();

    IEnumerator<IClient> IEnumerable<IClient>.GetEnumerator() => _clientList.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _clientList.GetEnumerator();

    private void Client_Disposed(object? sender, EventArgs e)
    {
        if (sender is Client client)
        {
            RemoveClient(client);
        }
    }

    private void InsertClient(Client client)
    {
        lock (LockObject)
        {
            var action = NotifyCollectionChangedAction.Add;
            var index = _clientList.Count;
            var args = new NotifyCollectionChangedEventArgs(action, client, index);
            _clientList.Add(client);
            _logger.LogDebug("Client is inserted into the collection: {Address}", client.Address);
            client.Disposed += Client_Disposed;
            CollectionChanged?.Invoke(this, args);
        }
    }

    private void RemoveClient(Client client)
    {
        lock (LockObject)
        {
            var action = NotifyCollectionChangedAction.Remove;
            var index = _clientList.IndexOf(client);
            var args = new NotifyCollectionChangedEventArgs(action, client, index);
            client.Disposed -= Client_Disposed;
            _clientList.RemoveAt(index);
            _logger.LogDebug("Client is removed from the collection: {Address}", client.Address);
            CollectionChanged?.Invoke(this, args);
            if (_current == client)
            {
                Current = _clientList.FirstOrDefault();
            }
        }
    }
}

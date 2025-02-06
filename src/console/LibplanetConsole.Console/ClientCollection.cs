using System.Collections;
using System.Collections.Specialized;
using LibplanetConsole.Alias;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Console;

internal sealed class ClientCollection(
    IServiceProvider serviceProvider,
    IApplicationOptions options)
    : ConsoleContentBase("clients"), IEnumerable<Client>, IClientCollection
{
    private static readonly object LockObject = new();
    private readonly List<Client> _clientList = new(options.Clients.Length);
    private readonly ILogger _logger = serviceProvider.GetLogger<ClientCollection>();
    private Client? _current;

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
        InsertClient(client);

        if (options.ProcessOptions is { } processOptions)
        {
            await client.StartProcessAsync(processOptions, cancellationToken);
        }

        return client;
    }

    public async Task AttachAsync(
        AttachOptions options, CancellationToken cancellationToken)
    {
        var client = this[options.Address];
        if (client.IsAttached is true)
        {
            throw new InvalidOperationException("The node is already attached.");
        }

        client.Url = options.Url;
        await client.AttachAsync(cancellationToken);
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        for (var i = 0; i < _clientList.Count; i++)
        {
            try
            {
                var client = _clientList[i];
                if (options.ProcessOptions is { } processOptions)
                {
                    await client.StartProcessAsync(processOptions, cancellationToken);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while initializing client.");
            }
        }
    }

    async Task<IClient> IClientCollection.AddNewAsync(
        AddNewClientOptions newOptions, CancellationToken cancellationToken)
        => await AddNewAsync(newOptions, cancellationToken);

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

    IEnumerator<Client> IEnumerable<Client>.GetEnumerator()
        => _clientList.GetEnumerator();

    IEnumerator<IClient> IEnumerable<IClient>.GetEnumerator()
        => _clientList.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => _clientList.GetEnumerator();

    protected override async Task OnStartAsync(CancellationToken cancellationToken)
    {
        try
        {
            for (var i = 0; i < _clientList.Capacity; i++)
            {
                var client = ClientFactory.CreateNew(serviceProvider, options.Clients[i]);
                InsertClient(client);
            }

            Current = _clientList.FirstOrDefault();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while starting clients.");
        }

        await Task.CompletedTask;
    }

    protected override async Task OnStopAsync(CancellationToken cancellationToken)
    {
        for (var i = _clientList.Count - 1; i >= 0; i--)
        {
            var client = _clientList[i]!;
            client.Disposed -= Client_Disposed;
            await ClientFactory.DisposeScopeAsync(client);
            _logger.LogDebug("Disposed a client: {Address}", client.Address);
        }
    }

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
            var aliases = serviceProvider.GetRequiredService<AliasCollection>();
            var aliasInfo = new AliasInfo
            {
                Alias = $"client-{index}",
                Address = client.Address,
                Tags = ["client", "temp"],
            };
            aliases.Add(aliasInfo);
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
            var aliases = serviceProvider.GetRequiredService<AliasCollection>();
            client.Disposed -= Client_Disposed;
            _clientList.RemoveAt(index);
            aliases.Remove(client.Address);
            _logger.LogDebug("Client is removed from the collection: {Address}", client.Address);
            CollectionChanged?.Invoke(this, args);
            if (_current == client)
            {
                Current = _clientList.FirstOrDefault();
            }
        }
    }
}

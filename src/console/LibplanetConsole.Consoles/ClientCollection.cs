using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Net;
using Libplanet.Crypto;
using LibplanetConsole.Clients;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Exceptions;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.Consoles;

[Export]
[Export(typeof(IClientCollection))]
[Export(typeof(IApplicationService))]
[Dependency(typeof(NodeCollection))]
[method: ImportingConstructor]
internal sealed class ClientCollection(
    ApplicationBase application, ApplicationOptions options, NodeCollection nodes)
    : IEnumerable<Client>, IClientCollection, IApplicationService
{
    private static readonly object LockObject = new();
    private readonly ApplicationBase _application = application;
    private readonly ApplicationOptions _options = options;
    private readonly List<Client> _clientList = new(options.ClientCount);
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

    public bool Contains(Address address) => _clientList.Any(item => item.Address == address);

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

    public Task<Client> AddNewAsync(CancellationToken cancellationToken)
        => AddNewAsync(new(), cancellationToken);

    public async Task<Client> AddNewAsync(
        PrivateKey privateKey, CancellationToken cancellationToken)
    {
        var node = nodes.RandomNode();
        var clientOptions = new ClientOptions()
        {
            NodeEndPoint = node.EndPoint,
        };
        var endPoint = DnsEndPointUtility.Next();
        var container = _application.CreateChildContainer();
        _ = new ClientProcess(endPoint, privateKey);
        var client = CreateNew(container, privateKey, endPoint);
        await client.StartAsync(clientOptions, cancellationToken);
        InsertClient(client);
        return client;
    }

    public async Task<Client> AttachAsync(
        EndPoint endPoint, PrivateKey privateKey, CancellationToken cancellationToken)
    {
        var node = nodes.RandomNode();
        var clientOptions = new ClientOptions()
        {
            NodeEndPoint = node.EndPoint,
        };
        var container = _application.CreateChildContainer();
        var client = CreateNew(container, privateKey, endPoint);
        await client.StartAsync(clientOptions, cancellationToken);
        InsertClient(client);
        return client;
    }

    async Task IApplicationService.InitializeAsync(
        IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        if (_options.ClientCount > 0)
        {
            await Parallel.ForAsync(0, _options.ClientCount, cancellationToken, BodyAsync);
            Current = _clientList.FirstOrDefault();
        }

        async ValueTask BodyAsync(int index, CancellationToken cancellationToken)
        {
            var privateKey = new PrivateKey();
            await AddNewAsync(privateKey, cancellationToken);
        }
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);

        for (var i = _clientList.Count - 1; i >= 0; i--)
        {
            var item = _clientList[i]!;
            await item.DisposeAsync();
        }

        _isDisposed = true;
        GC.SuppressFinalize(this);
    }

    async Task<IClient> IClientCollection.AddNewAsync(
        PrivateKey privateKey, CancellationToken cancellationToken)
        => await AddNewAsync(privateKey, cancellationToken);

    async Task<IClient> IClientCollection.AttachAsync(
        EndPoint endPoint, PrivateKey privateKey, CancellationToken cancellationToken)
        => await AttachAsync(endPoint, privateKey, cancellationToken);

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

    private static Client CreateNew(
        CompositionContainer container, PrivateKey privateKey, EndPoint endPoint)
    {
        lock (LockObject)
        {
            return new Client(container, privateKey, endPoint);
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
            _clientList.Add(client);
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
            CollectionChanged?.Invoke(this, args);
            if (_current == client)
            {
                Current = _clientList.FirstOrDefault();
            }
        }
    }
}

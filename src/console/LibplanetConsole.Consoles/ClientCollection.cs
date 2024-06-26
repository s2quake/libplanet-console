using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Exceptions;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Frameworks;
using Serilog;

namespace LibplanetConsole.Consoles;

[Dependency(typeof(NodeCollection))]
[method: ImportingConstructor]
internal sealed class ClientCollection(
    ApplicationBase application, AppPrivateKey[] privateKeys)
    : IEnumerable<Client>, IClientCollection, IApplicationService, IAsyncDisposable
{
    private static readonly object LockObject = new();
    private readonly ApplicationBase _application = application;
    private readonly List<Client> _clientList = new(privateKeys.Length);
    private readonly ILogger _logger = application.GetService<ILogger>();
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

    public Client this[AppAddress address] => _clientList.Single(item => item.Address == address);

    IClient IClientCollection.this[int index] => this[index];

    IClient IClientCollection.this[AppAddress address] => this[address];

    public bool Contains(Client item) => _clientList.Contains(item);

    public bool Contains(AppAddress address) => _clientList.Exists(item => item.Address == address);

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

    public int IndexOf(AppAddress address)
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
        AddNewOptions options, CancellationToken cancellationToken)
    {
        var client = CreateNew(options.PrivateKey);
        if (options.NoProcess != true)
        {
            var clientProcess = client.CreateProcess();
            clientProcess.Detach = options.Detach;
            clientProcess.ManualStart = options.Detach != true;
            clientProcess.NewWindow = options.NewWindow;
            await clientProcess.StartAsync(cancellationToken: default);
        }

        if (options.NoProcess != true && options.Detach != true)
        {
            await client.AttachAsync(cancellationToken);
        }

        if (client.IsAttached == true && options.ManualStart != true)
        {
            var nodes = _application.GetService<NodeCollection>();
            var node = nodes.RandomNode();
            await client.StartAsync(node, cancellationToken);
        }

        InsertClient(client);
        return client;
    }

    async Task IApplicationService.InitializeAsync(
        IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var info = _application.Info;
        await Parallel.ForAsync(0, _clientList.Capacity, cancellationToken, BodyAsync);
        Current = _clientList.FirstOrDefault();

        async ValueTask BodyAsync(int index, CancellationToken cancellationToken)
        {
            var options = new AddNewOptions
            {
                PrivateKey = privateKeys[index],
                NoProcess = info.NoProcess,
                Detach = info.Detach,
                NewWindow = info.NewWindow,
                ManualStart = info.ManualStart,
            };
            await AddNewAsync(options, cancellationToken);
        }
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);

        for (var i = _clientList.Count - 1; i >= 0; i--)
        {
            var item = _clientList[i]!;
            await item.DisposeAsync();
            _logger.Debug("Disposed a client: {Address}", item.Address);
        }

        _isDisposed = true;
        GC.SuppressFinalize(this);
    }

    async Task<IClient> IClientCollection.AddNewAsync(
        AddNewOptions options, CancellationToken cancellationToken)
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

    IEnumerator<Client> IEnumerable<Client>.GetEnumerator()
        => _clientList.GetEnumerator();

    IEnumerator<IClient> IEnumerable<IClient>.GetEnumerator()
        => _clientList.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => _clientList.GetEnumerator();

    private Client CreateNew(AppPrivateKey privateKey)
    {
        lock (LockObject)
        {
            return new Client(_application, privateKey)
            {
                EndPoint = AppEndPoint.Next(),
            };
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
            _logger.Debug("Client is inserted into the collection: {Address}", client.Address);
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
            _logger.Debug("Client is removed from the collection: {Address}", client.Address);
            CollectionChanged?.Invoke(this, args);
            if (_current == client)
            {
                Current = _clientList.FirstOrDefault();
            }
        }
    }
}

using System.Collections;
using System.ComponentModel.Composition;
using System.Net;
using Libplanet.Crypto;
using LibplanetConsole.ClientServices;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Exceptions;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.Executable;

[Export]
[Export(typeof(IApplicationService))]
[Dependency(typeof(NodeCollection))]
[method: ImportingConstructor]
internal sealed class ClientCollection(ApplicationOptions options, NodeCollection nodes)
    : IEnumerable<IClient>, IApplicationService
{
    private static readonly object LockObject = new();
    private readonly ApplicationOptions _options = options;
    private readonly List<Client> _clientList = new(options.ClientCount);
    private IClient? _current;
    private bool _isDisposed;

    public event EventHandler? CurrentChanged;

    public IClient? Current
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

    public IClient this[int index] => _clientList[index];

    public IClient this[Address address] => _clientList.Single(item => item.Address == address);

    public bool Contains(IClient item) => _clientList.Contains(item);

    public bool Contains(Address address) => _clientList.Any(item => item.Address == address);

    public int IndexOf(IClient item)
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

    public Task<IClient> AddNewAsync(CancellationToken cancellationToken)
        => AddNewAsync(new(), cancellationToken);

    public async Task<IClient> AddNewAsync(
        PrivateKey privateKey, CancellationToken cancellationToken)
    {
        var node = nodes.RandomNode();
        var clientOptions = new ClientOptions()
        {
            NodeEndPoint = node.EndPoint,
        };
        var endPoint = DnsEndPointUtility.Next();
        _ = new ClientProcess(endPoint, privateKey);
        var client = new Client(privateKey, endPoint);
        await client.StartAsync(clientOptions, cancellationToken);
        lock (LockObject)
        {
            _clientList.Add(client);
        }

        client.Disposed += Client_Disposed;
        return client;
    }

    public async Task<IClient> AttachAsync(
        EndPoint endPoint, PrivateKey privateKey, CancellationToken cancellationToken)
    {
        var node = nodes.RandomNode();
        var clientOptions = new ClientOptions()
        {
            NodeEndPoint = node.EndPoint,
        };
        var client = new Client(privateKey, endPoint);
        await client.StartAsync(clientOptions, cancellationToken);
        _clientList.Add(client);
        if (_current is null)
        {
            Current = client;
        }

        return client;
    }

    async Task IApplicationService.InitializeAsync(
        IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        if (_options.ClientCount > 0)
        {
            await Parallel.ForAsync(0, _options.NodeCount, async (index, cancellationToken) =>
            {
                var privateKey = new PrivateKey();
                await AddNewAsync(privateKey, cancellationToken);
            });
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

    IEnumerator<IClient> IEnumerable<IClient>.GetEnumerator()
        => _clientList.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => _clientList.GetEnumerator();

    private void Client_Disposed(object? sender, EventArgs e)
    {
        if (sender is Client client)
        {
            _clientList.Remove(client);
            if (_current == client)
            {
                Current = _clientList.FirstOrDefault();
            }
        }
    }
}

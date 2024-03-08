using System.Collections;
using System.Collections.Immutable;
using System.ComponentModel.Composition;
using Libplanet.Crypto;
using Libplanet.Net;
using LibplanetConsole.Executable.Exceptions;

namespace LibplanetConsole.Executable;

[Export]
[Export(typeof(IApplicationService))]
sealed class SwarmHostCollection : IEnumerable<SwarmHost>, IApplicationService
{
    private SwarmHost _current;
    private readonly SwarmHost[] _swarmHosts;
    private readonly BoundPeer _seedPeer;
    private readonly BoundPeer _consensusSeedPeer;
    private bool _isDisposed;

    [ImportingConstructor]
    public SwarmHostCollection(ApplicationOptions options)
        : this(CreatePrivateKeys(options.SwarmCount), options.StorePath)
    {
    }

    public SwarmHostCollection()
        : this(CreatePrivateKeys(4), storePath: string.Empty)
    {
    }

    public SwarmHostCollection(PrivateKey[] validators)
        : this(validators, storePath: string.Empty)
    {
    }

    public SwarmHostCollection(PrivateKey[] validators, string storePath)
    {
        var validatorKeys = validators.Select(item => item.PublicKey).ToArray();
        var swarmHosts = new SwarmHost[validators.Length];
        var peers = new BoundPeer[validators.Length];
        var consensusPeers = new BoundPeer[validators.Length];
        for (var i = 0; i < validators.Length; i++)
        {
            var privateKey = validators[i];
            var peer = peers[i];
            var consensusPeer = consensusPeers[i];
            swarmHosts[i] = new SwarmHost($"Swarm{i}", privateKey, validatorKeys, storePath);
            peers[i] = swarmHosts[i].Peer;
            consensusPeers[i] = swarmHosts[i].ConsensusPeer;
        }
        _swarmHosts = swarmHosts;
        _current = swarmHosts[0];
        _seedPeer = peers[0];
        _consensusSeedPeer = consensusPeers[0];
    }

    public SwarmHost Current
    {
        get => _current;
        set
        {
            if (_swarmHosts.Contains(value) == false)
                throw new ArgumentException($"'{value}' is not included in the collection.", nameof(value));
            _current = value;
            CurrentChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public int Count => _swarmHosts.Length;

    public SwarmHost this[int index] => _swarmHosts[index];

    public bool Contains(SwarmHost item) => _swarmHosts.Contains(item);

    public int IndexOf(SwarmHost item)
    {
        for (var i = 0; i < _swarmHosts.Length; i++)
        {
            if (Equals(item, _swarmHosts[i]) == true)
                return i;
        }
        return -1;
    }

    public event EventHandler? CurrentChanged;

    private static PrivateKey[] CreatePrivateKeys(int count)
    {
        var keyList = new List<PrivateKey>(count);
        for (var i = 0; i < count; i++)
        {
            keyList.Add(PrivateKeyUtility.Create($"Swarm{i}"));
        }
        return [.. keyList];
    }

    #region IApplicationService

    async Task IApplicationService.InitializeAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        await Task.WhenAll(_swarmHosts.Select(item => item.StartAsync(_seedPeer, _consensusSeedPeer, cancellationToken)));
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        ObjectDisposedExceptionUtility.ThrowIf(_isDisposed, this);

        for (var i = _swarmHosts.Length - 1; i >= 0; i--)
        {
            var item = _swarmHosts[i]!;
            await item.DisposeAsync();
        }
        _isDisposed = true;
        GC.SuppressFinalize(this);
    }

    #endregion

    #region IEnumerable

    IEnumerator<SwarmHost> IEnumerable<SwarmHost>.GetEnumerator()
    {
        foreach (var item in _swarmHosts)
        {
            yield return item;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => _swarmHosts.GetEnumerator();

    #endregion
}

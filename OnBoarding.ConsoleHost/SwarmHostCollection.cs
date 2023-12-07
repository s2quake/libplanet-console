using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Diagnostics;
using Libplanet.Blockchain;
using Libplanet.Crypto;

namespace OnBoarding.ConsoleHost;

[Export]
sealed class SwarmHostCollection : IEnumerable<SwarmHost>, IAsyncDisposable
{
    private readonly OrderedDictionary _itemById = [];
    private bool _isDisposed;

    public int Count => _itemById.Count;

    public SwarmHost this[int index] => (SwarmHost)_itemById[index]!;

    public SwarmHost this[string key] => (SwarmHost)_itemById[key]!;

    public SwarmHost AddNew(PrivateKey privateKey, BlockChain blockChain, PublicKey[] publicKeys)
    {
        ObjectDisposedException.ThrowIf(condition: _isDisposed, this);

        var swarmHost = new SwarmHost(privateKey, blockChain, publicKeys);
        _itemById.Add(swarmHost.Key, swarmHost);
        swarmHost.Disposed += Item_Disposed;
        return swarmHost;
    }

    public async ValueTask DisposeAsync()
    {
        ObjectDisposedException.ThrowIf(condition: _isDisposed, this);

        for (var i = _itemById.Count - 1; i >= 0; i--)
        {
            var item = (SwarmHost)_itemById[i]!;
            item.Disposed -= Item_Disposed;
            await item.DisposeAsync();
        }
        _itemById.Clear();
        _isDisposed = true;
        GC.SuppressFinalize(this);
    }

    public void Add(SwarmHost item)
    {
        ObjectDisposedException.ThrowIf(condition: _isDisposed, this);
        if (item.IsDisposed == true)
            throw new ArgumentException($"{nameof(item)} has already been disposed.");
        if (_itemById.Contains(item.Key) == true)
            throw new ArgumentException($"{nameof(item)} has already been included in collection");

        _itemById.Add(item.Key, item);
        item.Disposed += Item_Disposed;
    }

    public bool Contains(SwarmHost item) => _itemById.Contains(item.Key);

    public int IndexOf(SwarmHost item)
    {
        for (var i = 0; i < _itemById.Count; i++)
        {
            if (object.Equals(item, _itemById[i]) == true)
                return i;
        }
        return -1;
    }

    public async Task InitializeAsync(Application application, CancellationToken cancellationToken)
    {
        if (application.GetService<UserCollection>() is { } users)
        {
            var taskList = new List<Task>(users.Count);
            foreach (var item in users)
            {
                var blockChain = BlockChainUtils.CreateBlockChain(user: item, [.. users]);
                var publicKeys = users.Where(i => i != item).Select(i => i.PublicKey).ToArray();
                var swarmHost = AddNew(item.PrivateKey, blockChain, publicKeys);
                taskList.Add(swarmHost.StartAsync(cancellationToken));
            }
            await Task.WhenAll(taskList);
            var first = this.First();
            var s1 = this[1];
            var s2 = this[2];
            await s1.Target.AddPeersAsync([first.Target.AsPeer], TimeSpan.FromSeconds(1));
            await s2.Target.AddPeersAsync([first.Target.AsPeer], TimeSpan.FromSeconds(1));
            // await this.First().TestAsync(this);
            // await Task.WhenAll(this.Select(item => item.TestAsync(this)));
        }
        else
        {
            throw new UnreachableException();
        }
    }

    private void Item_Disposed(object? sender, EventArgs e)
    {
        if (sender is SwarmHost item)
        {
            item.Disposed -= Item_Disposed;
            _itemById.Remove(item.Key);
        }
    }

    #region IEnumerable

    IEnumerator<SwarmHost> IEnumerable<SwarmHost>.GetEnumerator()
    {
        foreach (var item in _itemById.Values)
        {
            if (item is SwarmHost obj)
            {
                yield return obj;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => _itemById.Values.GetEnumerator();

    #endregion
}

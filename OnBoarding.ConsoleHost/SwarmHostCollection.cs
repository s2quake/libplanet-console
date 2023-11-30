using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
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

    public SwarmHost AddNew(PrivateKey privateKey, BlockChain blockChain)
    {
        ObjectDisposedException.ThrowIf(condition: _isDisposed, this);

        var swarmHost = new SwarmHost(privateKey, blockChain);
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

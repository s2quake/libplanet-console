using System.Collections;
using Libplanet.Action;

namespace OnBoarding.ConsoleHost;

sealed class ActionCollection : IEnumerable<IAction>
{
    private readonly List<IAction> _itemList = [];

    public int Count => _itemList.Count;

    public IAction this[int index] => _itemList[index];

    public void Add(IAction item)
    {
        if (_itemList.Contains(item) == true)
            throw new ArgumentException($"{item} has already been included in collection");
        _itemList.Add(item);
    }

    public void Clear() => _itemList.Clear();

    #region IEnumerable

    IEnumerator<IAction> IEnumerable<IAction>.GetEnumerator() => _itemList.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _itemList.GetEnumerator();

    #endregion
}

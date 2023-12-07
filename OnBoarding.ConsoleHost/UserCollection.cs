using System.Collections;
using System.ComponentModel.Composition;

namespace OnBoarding.ConsoleHost;

[Export]
sealed class UserCollection : IEnumerable<User>
{
    private readonly List<User> _itemList = [new(0), new(1), new(2)];

    public int Count => _itemList.Count;

    public User this[int index] => _itemList[index];

    public User AddNew()
    {
        var item = new User(Count);
        _itemList.Add(item);
        return item;
    }

    #region IEnumerable

    IEnumerator<User> IEnumerable<User>.GetEnumerator() => _itemList.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _itemList.GetEnumerator();

    #endregion
}

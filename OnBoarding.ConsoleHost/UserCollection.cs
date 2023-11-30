using System.Collections;
using System.ComponentModel.Composition;

namespace OnBoarding.ConsoleHost;

[Export]
sealed class UserCollection : IEnumerable<User>
{
    private readonly List<User> _itemList = [new(), new(), new()];

    public int Count => _itemList.Count;

    public User this[int index] => _itemList[index];

    public User AddNew()
    {
        var item = new User();
        _itemList.Add(item);
        return item;
    }

    public void Add(User item)
    {
        if (_itemList.Contains(item) == true)
            throw new ArgumentException($"{item} has already been included in collection");
        _itemList.Add(item);
    }

    #region IEnumerable

    IEnumerator<User> IEnumerable<User>.GetEnumerator() => _itemList.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _itemList.GetEnumerator();

    #endregion
}

using System.Collections;
using System.ComponentModel.Composition;

namespace OnBoarding.ConsoleHost;

[Export]
sealed class UserCollection : IEnumerable<User>
{
    private readonly List<User> _itemList;
    private User _currentUser;

    public UserCollection()
        : this(ApplicationOptions.DefaultUserCount)
    {
    }

    public UserCollection(int count)
    {
        _itemList = new(count);
        for (var i = 0; i < _itemList.Capacity; i++)
        {
            _itemList.Add(new(name: $"User{i}"));
        }
        _currentUser = _itemList.First();
    }

    [ImportingConstructor]
    public UserCollection(ApplicationOptions options)
        : this(options.UserCount)
    {
    }

    public int Count => _itemList.Count;

    public User this[int index] => _itemList[index];

    public User CurrentUser
    {
        get => _currentUser;
        set
        {
            if (_itemList.Contains(value) == false)
                throw new ArgumentException($"'{value}' is not included in the collection.", nameof(value));
            _currentUser = value;
        }
    }

    public int IndexOf(User item) => _itemList.IndexOf(item);

    #region IEnumerable

    IEnumerator<User> IEnumerable<User>.GetEnumerator() => _itemList.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _itemList.GetEnumerator();

    #endregion
}

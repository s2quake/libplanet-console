using System.Collections;
using System.ComponentModel.Composition;
using System.Net;
using System.Net.Sockets;

namespace OnBoarding.ConsoleHost;

[Export]
sealed class UserCollection : IEnumerable<User>
{
    private const int UserCount = 4;
    private static readonly Queue<int> PortQueue = new(GetRandomUnusedPorts(UserCount * 2));
    private readonly List<User> _itemList;

    public UserCollection()
    {
        _itemList = new(UserCount);
        for (var i = 0; i < _itemList.Capacity; i++)
        {
            _itemList.Add(new(
                name: $"User{i}",
                peerPort: PortQueue.Dequeue(),
                consensusPeerPort: PortQueue.Dequeue()
            ));
        }
    }

    public int Count => _itemList.Count;

    public User this[int index] => _itemList[index];

    // public User AddNew(string name)
    // {
    //     var item = new User(name);
    //     _itemList.Add(item);
    //     return item;
    // }

    private static int[] GetRandomUnusedPorts(int count)
    {
        var ports = new int[count];
        var listeners = new TcpListener[count];
        for (var i = 0; i < count; i++)
        {
            listeners[i] = CreateListener();
            ports[i] = ((IPEndPoint)listeners[i].LocalEndpoint).Port;
        }
        for (var i = 0; i < count; i++)
        {
            listeners[i].Stop();
        }
        return ports;

        static TcpListener CreateListener()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            return listener;
        }
    }

    #region IEnumerable

    IEnumerator<User> IEnumerable<User>.GetEnumerator() => _itemList.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _itemList.GetEnumerator();

    #endregion
}

namespace LibplanetConsole.Common;

public sealed class PortGroup
{
    public const int DefaultSpace = 10;
    private readonly int _startingPort;
    private readonly int[] _ports;

    internal PortGroup(int startingPort, int count)
    {
        _startingPort = startingPort;
        _ports = new int[count];
        for (int i = 0; i < count; i++)
        {
            _ports[i] = GetPort(i);
        }
    }

    public int this[int index] => _ports[index];

    private int GetPort(int index)
    {
        if (_startingPort == 0)
        {
            return PortUtility.NextPort();
        }

        return PortUtility.ReservePort(_startingPort + index);
    }
}

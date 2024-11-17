namespace LibplanetConsole.Common;

public sealed class PortGenerator(int startingPort)
{
    public const int DefaultSpace = 10;
    private int _startingPort = startingPort;

    public int Space { get; init; } = DefaultSpace;

    public PortGroup Next() => Next(DefaultSpace);

    public PortGroup Next(int count)
    {
        var portGroup = new PortGroup(_startingPort, count);
        _startingPort = _startingPort is 0 ? 0 : _startingPort + count;

        return portGroup;
    }
}

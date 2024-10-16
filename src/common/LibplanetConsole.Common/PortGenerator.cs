namespace LibplanetConsole.Common;

public sealed class PortGenerator
{
    public const int DefaultSpace = 10;
    private readonly List<int> _portList = [];

    public PortGenerator(int startingPort)
    {
        _portList.Add(startingPort == 0 ? PortUtility.NextPort() : startingPort);
        Current = _portList[0];
    }

    public int Space { get; init; } = DefaultSpace;

    public int Current { get; private set; }

    public PortGenerationMode Mode { get; init; } = PortGenerationMode.Random;

    public int Next()
    {
        Current = GetPort(Current);
        _portList.Add(Current);
        _portList.Sort();
        return Current;
    }

    private int GetPort(int nextPort)
    {
        if (Mode == PortGenerationMode.Random)
        {
            var port = PortUtility.NextPort();
            while (IsValidRandomPort(port) is false)
            {
                port = PortUtility.NextPort();
            }

            return port;
        }

        return nextPort + Space;
    }

    private bool IsValidRandomPort(int randomPort)
    {
        for (var i = 0; i < _portList.Count; i++)
        {
            var port = _portList[i];
            if (Math.Abs(port - randomPort) < Space)
            {
                return false;
            }
        }

        return true;
    }
}

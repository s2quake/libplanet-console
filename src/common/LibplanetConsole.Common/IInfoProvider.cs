namespace LibplanetConsole.Common;

public interface IInfoProvider
{
    Type DeclaringType { get; }

    IEnumerable<(string Name, object? Value)> GetInfos();
}

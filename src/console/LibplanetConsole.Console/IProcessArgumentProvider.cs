namespace LibplanetConsole.Consoles;

public interface IProcessArgumentProvider
{
    bool CanSupport(Type type);

    IEnumerable<string> GetArguments(object obj);
}

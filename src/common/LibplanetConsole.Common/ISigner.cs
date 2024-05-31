namespace LibplanetConsole.Common;

public interface ISigner
{
    byte[] Sign(object obj);
}

namespace LibplanetConsole.Common;

public interface IVerifier
{
    bool Verify(object obj, byte[] signature);
}

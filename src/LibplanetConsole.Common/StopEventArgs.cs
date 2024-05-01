namespace LibplanetConsole.Common;

public sealed class StopEventArgs(StopReason reason) : EventArgs
{
    public StopReason Reason { get; } = reason;
}

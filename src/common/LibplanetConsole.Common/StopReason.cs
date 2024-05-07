namespace LibplanetConsole.Common;

public enum StopReason
{
    /// <summary>
    /// When the service shut down normally.
    /// </summary>
    None,

    /// <summary>
    /// When the server is shut down.
    /// </summary>
    Disconnected,

    /// <summary>
    /// When the server disconnects for unknown reasons.
    /// </summary>
    Faulted,
}

using JSSoft.Terminals;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Client.Executable.Tracers;

internal sealed class ClientEventTracer : IHostedService, IDisposable
{
    private readonly ApplicationOptions _options;
    private readonly IClient _client;

    public ClientEventTracer(ApplicationOptions options, IClient client)
    {
        _options = options;
        _client = client;
        _client.Started += Client_Started;
        _client.Stopped += Client_Stopped;
    }

    public Task StartAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;

    void IDisposable.Dispose()
    {
        _client.Started -= Client_Started;
        _client.Stopped -= Client_Stopped;
    }

    private void Client_Started(object? sender, EventArgs e)
    {
        var endPoint = _options.EndPoint;
        var message = $"BlockChain has been started.: {endPoint}";
        Console.Out.WriteColoredLine(message, TerminalColorType.BrightGreen);
    }

    private void Client_Stopped(object? sender, EventArgs e)
    {
        var endPoint = _options.EndPoint;
        var message = $"BlockChain has been stopped.: {endPoint}";
        Console.Out.WriteColoredLine(message, TerminalColorType.BrightGreen);
    }
}

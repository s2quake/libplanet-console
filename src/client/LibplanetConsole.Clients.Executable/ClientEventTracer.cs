using System.ComponentModel.Composition;
using JSSoft.Terminals;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.Clients.Executable;

[Export(typeof(IApplicationService))]
[method: ImportingConstructor]
internal sealed class ClientEventTracer(IClient client)
    : IApplicationService
{
    public Task InitializeAsync(
        IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        client.BlockAppended += Client_BlockAppended;
        client.Started += Client_Started;
        client.Stopped += Client_Stopped;
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        client.BlockAppended -= Client_BlockAppended;
        client.Started -= Client_Started;
        client.Stopped -= Client_Stopped;
        return ValueTask.CompletedTask;
    }

    private void Client_BlockAppended(object? sender, BlockEventArgs e)
    {
        var blockInfo = e.BlockInfo;
        var hash = blockInfo.Hash;
        var miner = blockInfo.Miner;
        var message = $"Block #{blockInfo.Height} '{hash:S}' Appended by '{miner:S}'";
        Console.Out.WriteColoredLine(message, TerminalColorType.BrightGreen);
    }

    private void Client_Started(object? sender, EventArgs e)
    {
        var message = $"Client has been started.";
        Console.Out.WriteColoredLine(message, TerminalColorType.BrightGreen);
        Console.Out.WriteLineAsJson(client.Info);
    }

    private void Client_Stopped(object? sender, StopEventArgs e)
    {
        var message = $"Client has been stopped: {e.Reason}.";
        Console.Out.WriteColoredLine(message, TerminalColorType.BrightGreen);
    }
}

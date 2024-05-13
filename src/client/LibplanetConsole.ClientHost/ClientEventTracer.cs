using System.ComponentModel.Composition;
using JSSoft.Terminals;
using LibplanetConsole.Clients;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Frameworks;
using LibplanetConsole.Nodes;

namespace LibplanetConsole.ClientHost;

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
        var hash = blockInfo.Hash[0..8];
        var miner = blockInfo.Miner[0..8];
        var message = $"Block #{blockInfo.Index} '{hash}' Appended by '{miner}'";
        Console.WriteLine(
            TerminalStringBuilder.GetString(message, TerminalColorType.BrightGreen));
    }

    private void Client_Started(object? sender, EventArgs e)
    {
        var message = $"Client has been started.";
        Console.WriteLine(
            TerminalStringBuilder.GetString(message, TerminalColorType.BrightGreen));
        Console.Out.WriteLineAsJson(client.Info);
    }

    private void Client_Stopped(object? sender, EventArgs e)
    {
        var message = $"BlockChain has been stopped.";
        Console.WriteLine(
            TerminalStringBuilder.GetString(message, TerminalColorType.BrightGreen));
    }
}

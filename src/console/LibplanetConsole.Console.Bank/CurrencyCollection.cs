namespace LibplanetConsole.Console.Bank;

internal sealed class CurrencyCollection : CurrencyCollectionBase, IConsoleContent
{
    private readonly RunningNode _runningNode;

    public CurrencyCollection(RunningNode runningNode)
    {
        _runningNode = runningNode;
        _runningNode.Started += RunningNode_Started;
        _runningNode.Stopped += RunningNode_Stopped;
    }

    IEnumerable<IConsoleContent> IConsoleContent.Dependencies { get; } = [];

    string IConsoleContent.Name => "currencies";

    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private async void RunningNode_Started(object? sender, EventArgs e)
    {
        var nodeBank = _runningNode.Node.GetRequiredKeyedService<NodeBank>(INode.Key);
        var addressInfos = await nodeBank.GetCurrenciesAsync(default);
        foreach (var addressInfo in addressInfos)
        {
            Add(addressInfo.Code, addressInfo.Currency);
        }
    }

    private void RunningNode_Stopped(object? sender, EventArgs e)
    {
        Clear();
    }
}

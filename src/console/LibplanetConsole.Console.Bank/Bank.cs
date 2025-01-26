using LibplanetConsole.Bank.Actions;

namespace LibplanetConsole.Console.Bank;

internal sealed class Bank : ConsoleContentBase, IBank
{
    private readonly IConsole _console;
    private readonly RunningNode _runningNode;
    private NodeBank? _nodeBank;

    public Bank(IConsole console, RunningNode runningNode)
        : base("bank")
    {
        _runningNode = runningNode;
        _runningNode.Started += RunningNode_Started;
        _runningNode.Stopped += RunningNode_Stopped;
        _console = console;
    }

    public async Task TransferAsync(
        Address recipientAddress,
        FungibleAssetValue amount,
        CancellationToken cancellationToken)
    {
        var senderAddress = _console.Address;
        var actions = new IAction[]
        {
            new TransferAction(senderAddress, recipientAddress, amount),
        };

        await _console.SendTransactionAsync(actions, cancellationToken);
    }

    public async Task<FungibleAssetValue> GetBalanceAsync(
        Address address, Currency currency, CancellationToken cancellationToken)
    {
        if (_nodeBank is null)
        {
            throw new InvalidOperationException("Bank service is not available.");
        }

        return await _nodeBank.GetBalanceAsync(address, currency, cancellationToken);
    }

    private void RunningNode_Started(object? sender, EventArgs e)
    {
        _nodeBank = _runningNode.Node.GetRequiredKeyedService<NodeBank>(INode.Key);
    }

    private void RunningNode_Stopped(object? sender, EventArgs e)
    {
        _nodeBank = null;
    }
}

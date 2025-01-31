using System.Text;
using JSSoft.Commands.Extensions;
using JSSoft.Terminals;
using LibplanetConsole.BlockChain;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Node.Executable;

internal sealed class SystemTerminal : SystemTerminalBase
{
    private const string PromptText = "libplanet-node $ ";
    private readonly SynchronizationContext _synchronizationContext;
    private readonly CommandContext _commandContext;
    private readonly IBlockChain _blockChain;
    private readonly ILogger<SystemTerminal> _logger;
    private BlockInfo _tip;
    private bool _isEnded;

    public SystemTerminal(
        IHostApplicationLifetime applicationLifetime,
        CommandContext commandContext,
        IBlockChain blockChain,
        SynchronizationContext synchronizationContext,
        ILogger<SystemTerminal> logger)
    {
        _synchronizationContext = synchronizationContext;
        _commandContext = commandContext;
        _commandContext.Owner = applicationLifetime;
        _logger = logger;
        _blockChain = blockChain;
        _blockChain.BlockAppended += BlockChain_BlockAppended;
        _blockChain.Started += BlockChain_Started;
        _blockChain.Stopped += BlockChain_Stopped;
        UpdateTip(_blockChain.Tip);
        applicationLifetime.ApplicationStopping.Register(() => _isEnded = true);
    }

    protected override void OnDispose()
    {
        _blockChain.Started -= BlockChain_Started;
        _blockChain.Stopped -= BlockChain_Stopped;
        _blockChain.BlockAppended -= BlockChain_BlockAppended;
        base.OnDispose();
    }

    protected override string FormatPrompt(string prompt)
    {
        var tip = _tip;
        if (_tip.Height == -1)
        {
            return prompt;
        }
        else
        {
            var tsb = new TerminalStringBuilder();
            tsb.AppendEnd();
            tsb.Append($"#{tip.Height} ");
            tsb.Foreground = TerminalColorType.BrightGreen;
            tsb.Append($"{tip.Hash.ToShortString()} ");
            tsb.ResetOptions();
            tsb.Append($"by ");
            tsb.Foreground = TerminalColorType.BrightGreen;
            tsb.Append($"{tip.Miner.ToShortString()}");
            tsb.ResetOptions();
            tsb.AppendEnd();
            return $"[{tsb}] {PromptText}";
        }
    }

    protected override string[] GetCompletions(string[] items, string find)
        => _commandContext.GetCompletions(items, find);

    protected override Task OnExecuteAsync(string command, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Executing command: {Command}", command);
        return _commandContext.ExecuteAsync(command, cancellationToken);
    }

    protected override void OnExecuted(Exception? exception)
    {
        base.OnExecuted(exception);
        if (exception is not null)
        {
            _logger.LogError(exception, "An error occurred while executing a command.");
        }
    }

    protected override void OnInitialize(TextWriter @out, TextWriter error)
    {
        _commandContext.Out = @out;
        _commandContext.Error = error;
    }

    private void BlockChain_BlockAppended(object? sender, BlockEventArgs e)
        => _synchronizationContext.Post(_ => UpdateTip(e.BlockInfo), null);

    private void BlockChain_Started(object? sender, EventArgs e)
        => _synchronizationContext.Post(_ => UpdateTip(_blockChain.Tip), null);

    private void BlockChain_Stopped(object? sender, EventArgs e)
        => _synchronizationContext.Post(_ => UpdateTip(_blockChain.Tip), null);

    private void UpdateTip(BlockInfo tip)
    {
        if (_isEnded is false)
        {
            _tip = tip;
            UpdatePrompt();
        }
    }

    private void UpdatePrompt()
    {
        var tip = _tip;
        if (tip.Height == -1)
        {
            Prompt = PromptText;
        }
        else
        {
            var sb = new StringBuilder();
            sb.Append($"#{tip.Height} ");
            sb.Append($"{tip.Hash.ToShortString()} ");
            sb.Append($"by ");
            sb.Append($"{tip.Miner.ToShortString()}");
            Prompt = $"[{sb}] {PromptText}";
        }
    }
}

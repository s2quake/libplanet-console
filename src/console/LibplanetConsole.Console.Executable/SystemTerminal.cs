using System.Text;
using JSSoft.Commands.Extensions;
using JSSoft.Terminals;
using LibplanetConsole.Blockchain;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Console.Executable;

internal sealed class SystemTerminal : SystemTerminalBase
{
    private const string PromptText = "libplanet-console $ ";
    private readonly SynchronizationContext _synchronizationContext;
    private readonly CommandContext _commandContext;
    private readonly IBlockChain _blockChain;
    private BlockInfo _tip;

    public SystemTerminal(
        IHostApplicationLifetime applicationLifetime,
        CommandContext commandContext,
        IBlockChain blockChain,
        SynchronizationContext synchronizationContext)
    {
        _synchronizationContext = synchronizationContext;
        _commandContext = commandContext;
        _commandContext.Owner = applicationLifetime;
        _blockChain = blockChain;
        _blockChain.BlockAppended += BlockChain_BlockAppended;
        _blockChain.Started += BlockChain_Started;
        _blockChain.Stopped += BlockChain_Stopped;
        UpdatePrompt(_blockChain.Tip);
        applicationLifetime.ApplicationStopping.Register(() => Prompt = "\u001b0");
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

    protected override string[] GetCompletion(string[] items, string find)
        => _commandContext.GetCompletion(items, find);

    protected override Task OnExecuteAsync(string command, CancellationToken cancellationToken)
        => _commandContext.ExecuteAsync(command, cancellationToken);

    protected override void OnInitialize(TextWriter @out, TextWriter error)
    {
        _commandContext.Out = @out;
        _commandContext.Error = error;
    }

    private void BlockChain_BlockAppended(object? sender, BlockEventArgs e)
        => _synchronizationContext.Post(_ => UpdatePrompt(e.BlockInfo), null);

    private void BlockChain_Started(object? sender, EventArgs e)
        => _synchronizationContext.Post(_ => UpdatePrompt(_blockChain.Tip), null);

    private void BlockChain_Stopped(object? sender, EventArgs e)
        => _synchronizationContext.Post(_ => UpdatePrompt(_blockChain.Tip), null);

    private void UpdatePrompt(BlockInfo tip)
    {
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

        _tip = tip;
    }
}

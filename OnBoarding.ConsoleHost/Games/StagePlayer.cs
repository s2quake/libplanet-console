using JSSoft.Library.Terminals;
using Libplanet.Action;
using OnBoarding.ConsoleHost.Actions;

namespace OnBoarding.ConsoleHost.Games;

sealed class StagePlayer(Stage stage, TextWriter @out)
{
    private readonly Stage _stage = stage;
    private readonly TextWriter _out = @out;

    public async Task<IAction[]> StartAsync(int tick, CancellationToken cancellationToken)
    {
        var turn = 0;
        var actionList = new List<IAction>();
        _stage.Player.LevelIncreased += Player_LevelIncreased;
        while (cancellationToken.IsCancellationRequested == false && _stage.IsEnded == false)
        {
            await _out.WriteLineAsync($"Turn #{turn}");
            _stage.Update();
            actionList.Add(new StageAction { StageInfo = (StageInfo)_stage });
            turn++;
            await Task.Delay(tick, cancellationToken: default);
        }
        if (cancellationToken.IsCancellationRequested == true)
        {
            throw new TaskCanceledException("Play has been canceled.");
        }
        return [.. actionList];
    }

    private void Player_LevelIncreased(object? sender, EventArgs e)
    {
        if (sender is Player player)
        {
            _out.WriteLine($"'{player.DisplayName}'" + TerminalStringBuilder.GetString($" level increased.: {player.Level}", TerminalColorType.BrightGreen));
        }
    }
}

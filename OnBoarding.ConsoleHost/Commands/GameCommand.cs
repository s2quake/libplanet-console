using System.ComponentModel.Composition;
using System.Text;
using Bencodex.Types;
using JSSoft.Library.Commands;
using Libplanet.Action.State;
using Libplanet.Blockchain;
using Libplanet.Crypto;
using OnBoarding.ConsoleHost.Actions;
using OnBoarding.ConsoleHost.Extensions;
using OnBoarding.ConsoleHost.Games;

namespace OnBoarding.ConsoleHost.Commands;

[Export(typeof(ICommand))]
[method: ImportingConstructor]
sealed class GameCommand(Application application) : CommandMethodBase
{
    [CommandProperty(InitValue = 1000)]
    public int Tick { get; set; }

    [CommandMethod]
    [CommandMethodProperty(nameof(Tick))]
    public async Task PlayAsync(CancellationToken cancellationToken)
    {
        var player = new Player(new PrivateKey().PublicKey, life: 1000);
        var monsters = MonsterCollection.Create(difficulty: 1, count: 10);
        var stage = new Stage(player, monsters);
        var turn = 0;
        while (cancellationToken.IsCancellationRequested == false && stage.IsEnded == false)
        {
            await Out.WriteLineAsync($"Turn #{turn}");
            stage.Update();
            turn++;
            await Task.Delay(Tick, cancellationToken: default);
        }
        if (cancellationToken.IsCancellationRequested == true)
        {
            Error.WriteLine("Play has been canceled.");
        }
    }
}

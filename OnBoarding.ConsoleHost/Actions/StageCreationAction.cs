using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Action.State;
using Libplanet.Crypto;
using OnBoarding.ConsoleHost.Games;

namespace OnBoarding.ConsoleHost.Actions;

[ActionType("stage-creation")]
sealed class StageCreationAction : ActionBase
{
    public Address StageAddress { get; set; }

    public int Difficulty { get; set; }

    public CharacterInfo Player { get; set; }

    protected override Dictionary OnInitialize(Dictionary values)
    {
        return values.Add(nameof(StageAddress), StageAddress.ToByteArray())
                     .Add(nameof(Difficulty), Difficulty)
                     .Add(nameof(Player), Player.ToBencodex());
    }

    protected override void OnLoadPlainValue(Dictionary values)
    {
        StageAddress = new Address(values[nameof(StageAddress)]);
        Difficulty = (Integer)values[nameof(Difficulty)];
        Player = CharacterInfo.FromBencodex(values[nameof(Player)]);
    }
}

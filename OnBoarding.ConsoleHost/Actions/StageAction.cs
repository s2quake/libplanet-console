using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Action.State;
using Libplanet.Crypto;
using OnBoarding.ConsoleHost.Games;

namespace OnBoarding.ConsoleHost.Actions;

[ActionType("stage")]
sealed class StageAction : ActionBase
{
    public StageInfo StageInfo { get; set; }

    protected override Dictionary OnInitialize(Dictionary values)
    {
        var monsters = Dictionary.Empty;
        for (var i = 0; i < StageInfo.Monsters.Length; i++)
        {
            monsters = monsters.Add($"{i}", GetCharacterValue(StageInfo.Monsters[i]));
        }
        return values.Add("turn", StageInfo.Turn)
                     .Add("player", GetCharacterValue(StageInfo.Player))
                     .Add("monsters", monsters);
    }

    protected override void OnLoadPlainValue(Dictionary values)
    {
        var monsterValues = (Dictionary)values["monsters"];
        var monsterInfos = new CharacterInfo[monsterValues.Count];
        for (var i = 0; i < monsterValues.Count; i++)
        {
            monsterInfos[i] = GetCharacterInfo((Dictionary)monsterValues[$"{i}"]);
        }
        StageInfo = new StageInfo
        {
            Turn = (Integer)values["turn"],
            Player = GetCharacterInfo((Dictionary)values["player"]),
            Monsters = monsterInfos,
        };
    }

    protected override IWorld OnExecute(IActionContext context)
    {
        var stageInfo = StageInfo;
        var previousState = context.PreviousState;
        var stageAccount = previousState.GetAccount(stageInfo.Player.Address);
        stageAccount = stageAccount.SetState(stageInfo.Player.Address, (Integer)stageInfo.Player.Life);
        return previousState.SetAccount(stageInfo.Player.Address, stageAccount);
    }

    private static Dictionary GetCharacterValue(CharacterInfo characterInfo)
    {
        return Dictionary.Empty.Add("address", characterInfo.Address.ByteArray)
                               .Add("life", characterInfo.Life);
    }

    private static CharacterInfo GetCharacterInfo(Dictionary values)
    {
        return new CharacterInfo
        {
            Address = new Address(values["address"]),
            Life = (Integer)values["life"],
        };
    }
}

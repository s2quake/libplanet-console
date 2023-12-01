using Libplanet.Crypto;

namespace OnBoarding.ConsoleHost.Games;

struct StageInfo
{
    public long Turn { get; set; }

    public Address Address { get; set; }

    public CharacterInfo[] Monsters { get; set; }

    public CharacterInfo Player { get; set; }

    public static explicit operator StageInfo(Stage stage)
    {
        return new StageInfo
        {
            Turn = stage.Turn,
            Address = stage.Address,
            Player = (CharacterInfo)stage.Player,
            Monsters = stage.Monsters.Select(item => (CharacterInfo)item).ToArray(),
        };
    }
}

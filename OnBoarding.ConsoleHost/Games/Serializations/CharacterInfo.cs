using Libplanet.Crypto;

namespace OnBoarding.ConsoleHost.Games;

struct CharacterInfo
{
    public Address Address { get; set; }

    public long Life { get; set; }

    public static explicit operator CharacterInfo(Character character)
    {
        return new CharacterInfo
        {
            Address = character.Address,
            Life = character.Life,
        };
    }
}

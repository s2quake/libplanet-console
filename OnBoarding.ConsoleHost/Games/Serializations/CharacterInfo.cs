using Bencodex.Types;
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

    public readonly IValue ToBencodex()
    {
        return Dictionary.Empty.Add(nameof(Address), Address.ToByteArray())
                               .Add(nameof(Life), Life);
    }

    public static CharacterInfo FromBencodex(IValue value)
    {
        if (value is Dictionary values)
        {
            return new()
            {
                Address = new Address(values[nameof(Address)]),
                Life = (Integer)values[nameof(Life)],
            };
        }
        throw new ArgumentException($"'{value}' must be a '{typeof(Dictionary)}'", nameof(value));
    }
}

using Bencodex.Types;
using Libplanet.Crypto;

namespace OnBoarding.ConsoleHost.Games;

struct PlayerInfo
{
    public static PlayerInfo Empty { get; } = new PlayerInfo();

    public Address Address { get; set; }

    public long Life { get; set; }

    public long MaxLife { get; set; }

    public long Experience { get; set; }

    public long Level { get; set; }

    public static explicit operator PlayerInfo(Player player)
    {
        return new PlayerInfo
        {
            Address = player.Address,
            Life = player.Life,
            MaxLife = player.MaxLife,
            Level = player.Level,
            Experience = player.Experience,
        };
    }

    public readonly IValue ToBencodex()
    {
        return Dictionary.Empty.Add(nameof(Address), Address.ToByteArray())
                               .Add(nameof(Life), Life)
                               .Add(nameof(MaxLife), MaxLife)
                               .Add(nameof(Experience), Experience)
                               .Add(nameof(Level), Level);
    }

    public static PlayerInfo FromBencodex(IValue value)
    {
        if (value is Dictionary values)
        {
            return new()
            {
                Address = new Address(values[nameof(Address)]),
                Life = (Integer)values[nameof(Life)],
                MaxLife = (Integer)values[nameof(MaxLife)],
                Experience = (Integer)values[nameof(Experience)],
                Level = (Integer)values[nameof(Level)],
            };
        }
        throw new ArgumentException($"'{value}' must be a '{typeof(Dictionary)}'", nameof(value));
    }
}

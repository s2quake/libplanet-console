using Bencodex.Types;
using JSSoft.Library.Terminals;
using Libplanet.Blockchain;
using Libplanet.Crypto;
using OnBoarding.ConsoleHost.Games.Serializations;

namespace OnBoarding.ConsoleHost.Games;

sealed class Player : Character
{
    private readonly Address _address;
    private long _experience;
    private long _level;

    public Player(PlayerInfo playerInfo)
        : base(playerInfo)
    {
        _address = playerInfo.Address;
        _experience = playerInfo.Experience;
        _level = playerInfo.Level;
        MaxExperience = GetExperience(_level);
        Skills = playerInfo.Skills.Select(item => SkillFactory.Create(this, item)).ToArray();
        DisplayName = TerminalStringBuilder.GetString($"{this}", TerminalColorType.Blue);
    }

    // public static Address CurrentAddress { get; set; }

    public Address Address => _address;

    public override ISkill[] Skills { get; }

    public override string DisplayName { get; }

    public long MaxExperience { get; private set; }

    public long Experience
    {
        get => _experience;
        set
        {
            _experience += value;
            while (_experience >= MaxExperience)
            {
                _level++;
                _experience -= MaxExperience;
                Heal(MaxLife - Life);
                MaxExperience = GetExperience(_level);
                LevelIncreased?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public long Level => _level;

    public static long GetExperience(long level)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(level, 0);

        return (long)(Math.Pow(level, 2) * 100);
    }

    public override string ToString()
    {
        return $"P:{_address}"[..8];
    }

    public override bool IsEnemyOf(Character character) => character is Monster;

    public event EventHandler? LevelIncreased;
}
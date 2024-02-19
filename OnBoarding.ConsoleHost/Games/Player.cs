using JSSoft.Terminals;
using OnBoarding.ConsoleHost.Games.Serializations;

namespace OnBoarding.ConsoleHost.Games;

sealed class Player : Character
{
    private long _experience;
    private long _level;

    public Player(PlayerInfo playerInfo)
        : base(playerInfo)
    {
        _experience = playerInfo.Experience;
        _level = playerInfo.Level;
        MaxExperience = GetExperience(_level);
        Skills = playerInfo.Skills.Select(item => SkillFactory.Create(this, item)).ToArray();
        DisplayName = TerminalStringBuilder.GetString($"{Name}", TerminalColorType.BrightBlue);
    }

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
                Heal((int)(MaxLife * 0.1));
                MaxExperience = GetExperience(_level);
                LevelIncreased?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public long Level => _level;

    public static long GetExperience(long level)
    {
        if (level < 0)
            throw new ArgumentOutOfRangeException(nameof(level));

        return (long)(Math.Pow(level, 2) * 100);
    }

    public override bool IsEnemyOf(Character character) => character is Monster;

    public event EventHandler? LevelIncreased;
}

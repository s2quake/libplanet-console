using Bencodex.Types;

namespace LibplanetConsole.Executable.Games.Serializations;

record class CharacterInfo
{
    public CharacterInfo()
    {
    }

    public CharacterInfo(Dictionary values)
    {
        Name = (Text)values[nameof(Name)];
        Life = (Integer)values[nameof(Life)];
        MaxLife = (Integer)values[nameof(MaxLife)];
    }

    public CharacterInfo(Character character)
    {
        Name = character.Name;
        Life = character.Life;
        MaxLife = character.MaxLife;
    }

    public string Name { get; init; } = string.Empty;

    public long Life { get; init; }

    public long MaxLife { get; init; }

    public virtual Dictionary ToBencodex()
    {
        return Dictionary.Empty.Add(nameof(Name), Name)
                               .Add(nameof(Life), Life)
                               .Add(nameof(MaxLife), MaxLife);
    }
}

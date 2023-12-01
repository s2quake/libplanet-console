using System.Collections;
using System.ComponentModel.Composition;
using Libplanet.Action;
using Libplanet.Crypto;

namespace OnBoarding.ConsoleHost.Games;

sealed class Stage
{
    private readonly Player _player;
    private readonly MonsterCollection _monsters;

    public Stage(Player player, MonsterCollection monsters)
    {
        _player = player;
        _monsters = monsters;
    }

    public Player Player => _player;

    public MonsterCollection Monsters => _monsters;

    public long Tick { get; private set; }

    public bool IsEnded => _player.IsDead == true || _monsters.Count == 0;

    public IEnumerable<Character> Characters
    {
        get
        {
            yield return _player;
            foreach (var item in _monsters)
            {
                yield return item;
            }
        }
    }

    public void Update()
    {
        var monsters = _monsters.ToArray();
        UpdateSkills(_player);
        foreach (var item in monsters)
        {
            UpdateSkills(item);
        }
        Tick++;
    }

    private void UpdateSkills(Character character)
    {
        foreach (var item in character.Skills)
        {
            if (item.CanExecute(this) == true)
            {
                item.Execute(this);
            }
            item.Tick();
        }
    }
}

using Libplanet.Crypto;

namespace OnBoarding.ConsoleHost.Games;

sealed class Stage(Player player, MonsterCollection monsters)
{
    private readonly Player _player = player;
    private readonly MonsterCollection _monsters = monsters;

    public Address Address { get; } = new PrivateKey().ToAddress();

    public Player Player => _player;

    public MonsterCollection Monsters => _monsters;

    public long Turn { get; private set; }

    public bool IsEnded => _player.IsDead == true || _monsters.AliveCount == 0;

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
        Turn++;
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

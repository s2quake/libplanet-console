using System.Collections;
using System.ComponentModel.Composition;
using Libplanet.Action;
using Libplanet.Crypto;
using Microsoft.VisualBasic;

namespace OnBoarding.ConsoleHost.Games;

sealed class MonsterCollection : IEnumerable<Monster>
{
    private readonly List<Monster> _itemList;

    public MonsterCollection(int capacity)
    {
        _itemList = new(capacity);
    }

    public int Count => _itemList.Count;

    public int AliveCount { get; private set; }

    public Monster this[int index] => _itemList[index];

    public static MonsterCollection Create(int difficulty, int count)
    {
        var monsters = new MonsterCollection(count);
        for (var i = 0; i < count; i++)
        {
            var monster = new Monster(life: 10);
            monsters._itemList.Add(monster);
            monster.Dead += (s, e) => monsters.AliveCount--;
        }
        monsters.AliveCount = count;
        return monsters;
    }

    #region IEnumerable

    IEnumerator<Monster> IEnumerable<Monster>.GetEnumerator() => _itemList.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _itemList.GetEnumerator();

    #endregion
}

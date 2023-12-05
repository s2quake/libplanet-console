using System.Collections;
using System.ComponentModel.Composition;
using Libplanet.Action;
using Libplanet.Crypto;
using Microsoft.VisualBasic;

namespace OnBoarding.ConsoleHost.Games;

sealed class MonsterCollection : IEnumerable<Monster>
{
    private readonly List<Monster> _itemList;

    public MonsterCollection(MonsterInfo[] monsterInfos)
    {
        _itemList = new(monsterInfos.Length);
        for (var i = 0; i < monsterInfos.Length; i++)
        {
            var monsterInfo = monsterInfos[i];
            var monster = new Monster(monsterInfo);
            _itemList.Add(monster);
            monster.Dead += (s, e) => AliveCount--;
        }
        AliveCount = Count;
    }

    public int Count => _itemList.Count;

    public int AliveCount { get; private set; }

    public Monster this[int index] => _itemList[index];

    #region IEnumerable

    IEnumerator<Monster> IEnumerable<Monster>.GetEnumerator() => _itemList.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _itemList.GetEnumerator();

    #endregion
}

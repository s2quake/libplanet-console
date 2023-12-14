using System.Collections;
using Libplanet.Action.State;
using List = Bencodex.Types.List;
using Dictionary = Bencodex.Types.Dictionary;

namespace OnBoarding.ConsoleHost.Games.Serializations;

sealed class RankInfoCollection : IEnumerable<RankInfo>
{
    private readonly List<RankInfo> _itemList;

    public RankInfoCollection(IAccount account)
    {
        if (account.GetState(WorldStates.LeaderBoard) is List list)
        {
            var itemList = new List<RankInfo>(list.Count);
            foreach (var item in list)
            {
                itemList.Add(new RankInfo((Dictionary)item));
            }
            _itemList = itemList;
        }
        else
        {
            _itemList = new();
        }
    }

    public int Count => _itemList.Count;

    public RankInfo this[int index] => _itemList[index];

    public void Add(RankInfo rankInfo)
    {
        for (var i = 0; i < _itemList.Count; i++)
        {
            if (_itemList[i].Address == rankInfo.Address)
            {
                _itemList.RemoveAt(i);
                break;
            }
        }
        _itemList.Add(rankInfo);
        _itemList.Sort();
    }

    public void Slice(int count)
    {
        while (_itemList.Count > count)
        {
            _itemList.RemoveAt(_itemList.Count - 1);
        }
    }

    public List ToBencodex()
    {
        var list = List.Empty;
        foreach (var item in _itemList)
        {
            list = list.Add(item.ToBencodex());
        }
        return list;
    }

    #region IEnumerable

    IEnumerator<RankInfo> IEnumerable<RankInfo>.GetEnumerator() => _itemList.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _itemList.GetEnumerator();

    #endregion
}

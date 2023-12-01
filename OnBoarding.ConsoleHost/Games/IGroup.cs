using System.Collections;
using System.ComponentModel.Composition;
using Libplanet.Action;
using Libplanet.Crypto;

namespace OnBoarding.ConsoleHost.Games;

interface IGroup
{
    string Name { get; }

    IGroup[] Allies { get; }

    IGroup[] Enemies { get; }
}

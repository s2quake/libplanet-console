
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Action.State;

namespace OnBoarding.ConsoleHost;

sealed class DummyAction : IAction
{
    public IValue PlainValue => throw new NotImplementedException();

    public IWorld Execute(IActionContext context)
    {
        throw new NotImplementedException();
    }

    public void LoadPlainValue(IValue plainValue)
    {
        throw new NotImplementedException();
    }
}
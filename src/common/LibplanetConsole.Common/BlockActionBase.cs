using Libplanet.Action.State;

namespace LibplanetConsole.Common;

public abstract class BlockActionBase : IAction
{
    IValue IAction.PlainValue => Dictionary.Empty;

    void IAction.LoadPlainValue(IValue plainValue)
    {
    }

    IWorld IAction.Execute(IActionContext context) => OnExecute(context);

    protected abstract IWorld OnExecute(IActionContext context);
}

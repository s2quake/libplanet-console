using System.Reflection;
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Action.State;

namespace LibplanetConsole.Common;

public abstract class ActionBase : IAction
{
    private Dictionary? _values;

    protected ActionBase()
    {
        TypeId = GetType().GetCustomAttribute<ActionTypeAttribute>() is { } attribute
            ? attribute.TypeIdentifier
            : throw new InvalidOperationException(
                $"Given type {this.GetType()} is missing {nameof(ActionTypeAttribute)}.");
    }

    public IValue PlainValue
    {
        get
        {
            if (_values is null)
            {
                _values = Dictionary.Empty;
                _values = _values.Add("type_id", TypeId);
                _values = OnInitialize(_values);
            }

            return _values;
        }
    }

    public IValue TypeId { get; }

    void IAction.LoadPlainValue(IValue plainValue)
    {
        if (plainValue is Dictionary values)
        {
            OnLoadPlainValue(values);
        }
    }

    IWorld IAction.Execute(IActionContext context) => OnExecute(context);

    protected virtual Dictionary OnInitialize(Dictionary values) => values;

    protected virtual void OnLoadPlainValue(Dictionary values)
    {
    }

    protected virtual IWorld OnExecute(IActionContext context) => context.PreviousState;
}

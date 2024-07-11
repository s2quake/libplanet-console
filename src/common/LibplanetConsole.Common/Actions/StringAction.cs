using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Action.State;
using Libplanet.Crypto;

namespace LibplanetConsole.Common.Actions;

[ActionType("string")]
public sealed class StringAction : ActionBase
{
    public static readonly Address StringAddress
        = new("0x07D2D759D2d1EF1dE09337ffFD281EdF1cd0AAA3");

    public string Value { get; set; } = string.Empty;

    protected override void OnLoadPlainValue(Dictionary values) => Value = (Text)values["value"];

    protected override Dictionary OnInitialize(Dictionary values) => values.SetItem("value", Value);

    protected override IWorld OnExecute(IActionContext context)
    {
        var world = context.PreviousState;
        var account = world.GetAccount(context.Signer);

        var value = account.GetState(StringAddress);
        var data = Dictionary.Empty.Add("PrevValue", value ?? Null.Value)
                                   .Add("Value", Value);

        account = account.SetState(StringAddress, data);
        world = world.SetAccount(context.Signer, account);
        return world;
    }
}

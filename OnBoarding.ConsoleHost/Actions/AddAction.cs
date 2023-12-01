using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Action.State;
using Libplanet.Crypto;

namespace OnBoarding.ConsoleHost.Actions;

[ActionType("add")]
sealed class AddAction : ActionBase
{
    public int Value { get; set; }

    public Address Address { get; set; }

    protected override Dictionary OnInitialize(Dictionary values)
    {
        return values.Add("value", Value)
                     .Add("address", Address.ByteArray);
    }

    protected override void OnLoadPlainValue(Dictionary values)
    {
        base.OnLoadPlainValue(values);
        Value = (Integer)values["value"];
        Address = new Address(values["address"]);
    }

    protected override IWorld OnExecute(IActionContext context)
    {
        var previousState = context.PreviousState;
        var legacyAccount = previousState.GetAccount(ReservedAddresses.LegacyAccount);
        var currentValue = 0;

        if (legacyAccount.GetState(Address) is Integer integer)
        {
            currentValue = integer;
        }
        currentValue += Value;
        if (currentValue < 0)
            throw new SystemException($"Result Value is greater or equal then 0. current value is {currentValue}");
        legacyAccount = legacyAccount.SetState(Address, (Integer)currentValue);
        return previousState.SetAccount(ReservedAddresses.LegacyAccount, legacyAccount);
    }
}

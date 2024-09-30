using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Action.State;
using Libplanet.Crypto;
using Libplanet.Types.Assets;
using LibplanetConsole.Common;

namespace LibplanetConsole.Bank.Actions;

[ActionType(TypeIdentifier)]
public sealed class InitializeValidator : ActionBase
{
    private const string TypeIdentifier = "initialize_validator";

    public InitializeValidator(PublicKey validator, decimal amount)
    {
        Validator = validator;
        Amount = amount;
    }

    public InitializeValidator()
    {
        Validator = new PrivateKey().PublicKey;
    }

    public PublicKey Validator { get; set; }

    public decimal Amount { get; set; }

    public Currency Currency { get; set; } = AssetUtility.NCG;

    protected override Dictionary OnInitialize(Dictionary values)
    {
        return base.OnInitialize(values)
            .Add("validator", Validator.ToBencodex())
            .Add("amount", $"{Amount:R}")
            .Add("currency", Currency.Serialize());
    }

    protected override void OnLoadPlainValue(Dictionary values)
    {
        base.OnLoadPlainValue(values);
        Validator = PublicKey.FromBencodex(values["validator"]);
        Amount = decimal.Parse((Text)values["amount"]);
        Currency = new Currency(values["currency"]);
    }

    protected override IWorld OnExecute(IActionContext context)
    {
        GasTracer.UseGas(1);
        var world = context.PreviousState;

        return world;
    }
}

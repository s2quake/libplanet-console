using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Action.State;
using Libplanet.Crypto;
using Libplanet.Types.Assets;
using LibplanetConsole.Common;
using Nekoyume.Action.DPoS.Control;
using Nekoyume.Action.DPoS.Util;
using Nekoyume.Module;

namespace LibplanetConsole.Nodes.Banks.Actions;

[ActionType(ActionTypeValue)]
public sealed class InitializeValidator : ActionBase
{
    private const string ActionTypeValue = "initialize_validator";

    public InitializeValidator(PublicKey validator, FungibleAssetValue amount)
    {
        Validator = validator;
        Amount = amount;
    }

    public InitializeValidator()
    {
        Validator = new PrivateKey().PublicKey;
    }

    public PublicKey Validator { get; set; }

    public FungibleAssetValue Amount { get; set; }

    protected override Dictionary OnInitialize(Dictionary values)
    {
        return base.OnInitialize(values)
            .Add("validator", Validator.Serialize())
            .Add("amount", Amount.Serialize());
    }

    protected override void OnLoadPlainValue(Dictionary values)
    {
        base.OnLoadPlainValue(values);
        Validator = values["validator"].ToPublicKey();
        Amount = values["amount"].ToFungibleAssetValue();
    }

    protected override IWorld OnExecute(IActionContext context)
    {
        context.UseGas(1);
        var world = context.PreviousState;
        var nativeTokens = world.GetNativeTokens();

        world = ValidatorCtrl.Create(
            world,
            context,
            Validator.Address,
            Validator,
            Amount,
            nativeTokens);

        return world;
    }
}

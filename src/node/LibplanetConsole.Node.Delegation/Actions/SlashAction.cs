using Libplanet.Action.State;
using LibplanetConsole.Common;
using Nekoyume.Model.Guild;
using Nekoyume.ValidatorDelegation;

namespace LibplanetConsole.Node.Delegation.Actions;

[ActionType("delegation-slash-action")]
public sealed class SlashAction : ActionBase
{
    private const string SlashFactorKey = "slashFactor";

    public SlashAction()
    {
    }

    public SlashAction(long slashFactor)
    {
        SlashFactor = slashFactor;
    }

    public long SlashFactor { get; set; } = 10;

    protected override void OnLoadPlainValue(Dictionary values)
        => SlashFactor = (Integer)values[SlashFactorKey];

    protected override Dictionary OnInitialize(Dictionary values)
        => values.SetItem(SlashFactorKey, SlashFactor);

    protected override IWorld OnExecute(IActionContext context)
    {
        var world = context.PreviousState;
        var signer = context.Signer;
        var repository = new ValidatorRepository(world, context);
        var slashFactor = SlashFactor;
        var height = context.BlockIndex;
        var validatorDelegatee = repository.GetDelegatee(signer);
        if (validatorDelegatee.Jailed is true)
        {
            throw new InvalidOperationException("The validator is already jailed.");
        }

        validatorDelegatee.Slash(slashFactor, height, height);

        var guildRepository = new GuildRepository(repository.World, repository.ActionContext);
        var guildDelegatee = guildRepository.GetDelegatee(signer);
        guildDelegatee.Slash(slashFactor, height, height);
        repository.UpdateWorld(guildRepository.World);
        return repository.World;
    }
}

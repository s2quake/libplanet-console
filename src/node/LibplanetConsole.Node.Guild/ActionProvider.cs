using System.Collections.Immutable;
using System.Numerics;
using System.Reflection;
using Libplanet.Types.Consensus;
using LibplanetConsole.Common.Actions;
using LibplanetConsole.Node.Guild.BlockActions;
using Nekoyume;
using Nekoyume.Action;
using Nekoyume.Action.Loader;
using Nekoyume.Action.ValidatorDelegation;
using Nekoyume.Model.State;
using Nekoyume.TableData;
using static Nekoyume.Action.MintAssets;

namespace LibplanetConsole.Node.Guild;

internal sealed class ActionProvider : IActionProvider
{
    private static readonly BigInteger DefaultPower = BigInteger.Pow(10, 20);
    private readonly Dictionary<string, string> _sheets;

    public ActionProvider()
    {
        _sheets = Lib9c.DevExtensions.Utils.ImportSheets(".submodules/lib9c/Lib9c/TableCSV");
    }

    public ImmutableArray<IAction> BeginBlockActions { get; } =
    [
        new TransferGoldToRewardPool(),
        new SlashValidator(),
        new AllocateGuildReward(),
        new AllocateReward(),
    ];

    public ImmutableArray<IAction> EndBlockActions { get; } =
    [
        new UpdateValidators(),
        new RecordProposer(),
        new RewardGold(),
        new ReleaseValidatorUnbondings(),
    ];

    public ImmutableArray<IAction> BeginTxActions { get; } =
    [
        new Mortgage(),
    ];

    public ImmutableArray<IAction> EndTxActions { get; } =
    [
        new Reward(),
        new Refund(),
    ];

    public IActionLoader GetActionLoader()
    {
        var executingAssembly = Assembly.GetExecutingAssembly();
        var assemblies = GetAssemblies(executingAssembly);
        IActionLoader[] actionLoaders =
        [
            new NCActionLoader(),
            .. assemblies.Select(item => new AssemblyActionLoader(item)),
        ];
        return new AggregateTypedActionLoader(actionLoaders);
    }

    public IAction[] GetGenesisActions(Address genesisAddress, PublicKey[] validatorKeys)
    {
        var validators = validatorKeys
            .Select((item, i) => new Validator(item, i == 0 ? DefaultPower * 100 : DefaultPower))
            .ToArray();
        var gameConfigState = new GameConfigState(_sheets[nameof(GameConfigSheet)]);
        var redeemCodeListSheet = new RedeemCodeListSheet();
        var ncg = Currency.Uncapped("NCG", 2, null);
        var initializeStates = new InitializeStates(
            validatorSet: new ValidatorSet(validators.ToList()),
            rankingState: new RankingState0(),
            shopState: new ShopState(),
            tableSheets: _sheets,
            gameConfigState: gameConfigState,
            redeemCodeState: new RedeemCodeState(redeemCodeListSheet),
            adminAddressState: null,
            activatedAccountsState: new ActivatedAccountsState(ImmutableHashSet<Address>.Empty),
            goldCurrencyState: new GoldCurrencyState(ncg, 1_000_000_000_000),
            goldDistributions: [],
            pendingActivationStates: [])
        {
            AssetMinters = ImmutableHashSet.Create(genesisAddress),
        };

        MintSpec[] mintSpecs =
        [
            new MintSpec(genesisAddress, ncg * 1_00, null),
            .. validatorKeys.Select(item => new MintSpec(item.Address, ncg * 1_000, null)),
        ];
        var mintAsset = new MintAssets(mintSpecs, "Initialize");
        return [initializeStates, mintAsset];
    }

    private static IEnumerable<Assembly> GetAssemblies(Assembly assembly)
    {
        var directory = Path.GetDirectoryName(assembly.Location)!;
        var files = Directory.GetFiles(directory, "LibplanetConsole.*.dll");
        string[] paths =
        [
            assembly.Location,
            .. files,
        ];
        return [.. paths.Distinct().Order().Select(Assembly.LoadFrom)];
    }
}

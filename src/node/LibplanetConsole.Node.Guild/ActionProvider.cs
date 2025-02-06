using System.Collections.Immutable;
using System.Numerics;
using System.Reflection;
using Libplanet.Types.Consensus;
using LibplanetConsole.Common.Actions;
using LibplanetConsole.Node.Guild.BlockActions;
using Nekoyume.Action;
using Nekoyume.Action.Guild;
using Nekoyume.Action.Loader;
using Nekoyume.Action.ValidatorDelegation;
using Nekoyume.Model.State;
using Nekoyume.TableData;
using static Nekoyume.Action.MintAssets;

namespace LibplanetConsole.Node.Guild;

internal sealed class ActionProvider : IActionProvider
{
    private const long InitialSupply = 1_000_000_000_000;
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
            goldCurrencyState: new GoldCurrencyState(ncg, InitialSupply),
            goldDistributions: [],
            pendingActivationStates: [])
        {
            AssetMinters = ImmutableHashSet.Create(genesisAddress),
        };

        Address[] addresses =
        [
            genesisAddress,
            new Address("56a75a25f0a8614bC119309cEb61BeA30e35FF9e"), // client 1
            .. validatorKeys.Select(item => item.Address),
        ];

        var mintSpecs = addresses.Select(CreateMintSpec).ToArray();
        var mintAsset = new MintAssets(mintSpecs, "Initialize");
        var makeGuild = new MakeGuild(validatorKeys[0].Address);
        var createAvatar = new CreateAvatar { name = "Console" };
        return [initializeStates, mintAsset, makeGuild, createAvatar];

        MintSpec CreateMintSpec(Address address) => new(address, ncg * 1_000_000, null);
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

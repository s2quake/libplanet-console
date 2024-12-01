using System.Collections.Immutable;
using System.Numerics;
using System.Reflection;
using Libplanet.Types.Consensus;
using LibplanetConsole.Common.Actions;

namespace LibplanetConsole.Node;

internal sealed class ActionProvider : IActionProvider
{
    public static ActionProvider Default { get; } = new();

    public ImmutableArray<IAction> BeginBlockActions { get; } = [];

    public ImmutableArray<IAction> EndBlockActions { get; } = [];

    public ImmutableArray<IAction> BeginTxActions { get; } = [];

    public ImmutableArray<IAction> EndTxActions { get; } = [];

    public IAction[] GetGenesisActions(Address genesisAddress, PublicKey[] validatorKeys)
    {
        var validators = validatorKeys
            .Select(item => new Validator(item, BigInteger.One))
            .ToArray();
        var validatorSet = new ValidatorSet(validators: [.. validators]);
        var actions = new IAction[]
        {
            new Libplanet.Action.Sys.Initialize(
                validatorSet: validatorSet,
                states: ImmutableDictionary.Create<Address, IValue>()),
        };

        return actions;
    }

    public IActionLoader GetActionLoader()
    {
        var executingAssembly = Assembly.GetExecutingAssembly();
        var assemblies = GetAssemblies(executingAssembly);
        var actionLoaders = assemblies.Select(item => new AssemblyActionLoader(item)).ToArray();
        return new AggregateTypedActionLoader(actionLoaders);
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

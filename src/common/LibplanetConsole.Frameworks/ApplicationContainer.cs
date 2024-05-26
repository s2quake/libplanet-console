using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Data;
using System.Reflection;

namespace LibplanetConsole.Frameworks;

public sealed class ApplicationContainer : CompositionContainer, IAsyncDisposable
{
    private static readonly object _lock = new();
    private readonly HashSet<IAsyncDisposable> _instanceHash = [];
    private readonly List<IAsyncDisposable> _instanceList = [];
    private readonly object _owner;
    private readonly ApplicationContainer? _parentContainer;

    public ApplicationContainer(object owner)
        : base(GetCatalog(GetAssemblies(owner)))
    {
        _owner = owner;
    }

    public ApplicationContainer(
        object owner, ApplicationContainer container, params ExportProvider[] providers)
        : base(container.Catalog, providers)
    {
        _parentContainer = container;
        _owner = owner;
    }

    public static IEnumerable<Assembly> GetAssemblies(object owner)
        => GetAssemblies(owner.GetType().Assembly);

    public static IEnumerable<Assembly> GetAssemblies()
        => GetAssemblies(Assembly.GetEntryAssembly()!);

    public static IEnumerable<Assembly> GetAssemblies(Assembly assembly)
    {
        var directory = Path.GetDirectoryName(assembly.Location)!;
        var directoryCatalog = new DirectoryCatalog(directory, "LibplanetConsole.*.dll");
        string[] paths =
        [
            assembly.Location,
            .. directoryCatalog.LoadedFiles,
        ];
        return [.. paths.Distinct().Order().Select(Assembly.LoadFrom)];
    }

    public async ValueTask DisposeAsync()
    {
        base.Dispose();
        for (var i = _instanceList.Count - 1; i >= 0; i--)
        {
            await _instanceList[i].DisposeAsync();
        }
    }

    [Obsolete("Use DisposeAsync instead.")]
    public new void Dispose()
    {
        throw new NotSupportedException();
    }

    protected override IEnumerable<Export>? GetExportsCore(
        ImportDefinition definition, AtomicComposition? atomicComposition)
    {
        if (base.GetExportsCore(definition, atomicComposition) is { } exports)
        {
            foreach (var export in exports)
            {
                yield return new ApplicationExport(export, OnExport);
            }
        }
    }

    private static AggregateCatalog GetCatalog(IEnumerable<Assembly> assemblies)
    {
        var assemblyCatalogs = assemblies.Select(item => new AssemblyCatalog(item));
        return new AggregateCatalog([.. assemblyCatalogs]);
    }

    private void OnExport(IAsyncDisposable asyncDisposable)
    {
        lock (_lock)
        {
            if (ContainsExport(asyncDisposable) != true &&
                _parentContainer?.ContainsExport(asyncDisposable) != true)
            {
                _instanceHash.Add(asyncDisposable);
                _instanceList.Add(asyncDisposable);
            }
        }
    }

    private bool ContainsExport(IAsyncDisposable asyncDisposable)
    {
        if (_owner == asyncDisposable)
        {
            return true;
        }

        return _instanceHash.Contains(asyncDisposable);
    }
}

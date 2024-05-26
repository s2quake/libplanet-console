using System.Collections;

namespace LibplanetConsole.Frameworks;

public sealed class ApplicationServiceCollection(
    IEnumerable<IApplicationService> applicationServices)
    : IEnumerable<IApplicationService>
{
    private readonly IApplicationService[] _applicationServices = Sort(applicationServices);

    public async Task InitializeAsync(
        IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        for (var i = 0; i < _applicationServices.Length; i++)
        {
            await _applicationServices[i].InitializeAsync(serviceProvider, cancellationToken);
        }
    }

    IEnumerator<IApplicationService> IEnumerable<IApplicationService>.GetEnumerator()
        => _applicationServices.OfType<IApplicationService>().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _applicationServices.GetEnumerator();

    private static IApplicationService[] Sort(IEnumerable<IApplicationService> items)
    {
        return DependencyUtility.TopologicalSort(items, GetDependencies).ToArray();

        IEnumerable<IApplicationService> GetDependencies(IApplicationService item)
        {
            return DependencyUtility.GetDependencies(item, items);
        }
    }
}

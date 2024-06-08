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
            var serviceName = _applicationServices[i].GetType().Name;
            ApplicationLogger.Debug("Application service initializing: {0}", serviceName);
            await _applicationServices[i].InitializeAsync(serviceProvider, cancellationToken);
            ApplicationLogger.Debug("Application service initialized: {0}", serviceName);
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

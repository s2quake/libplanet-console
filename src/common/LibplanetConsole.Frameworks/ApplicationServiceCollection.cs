using System.Collections;
using JSSoft.Commands;
using JSSoft.Commands.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace LibplanetConsole.Frameworks;

public sealed class ApplicationServiceCollection(
    IEnumerable<IApplicationService> applicationServices)
    : IEnumerable<IApplicationService>
{
    private readonly IApplicationService[] _applicationServices = Sort(applicationServices);

    public async Task InitializeAsync(
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken,
        IProgress<ProgressInfo> progress)
    {
        var logger = serviceProvider.GetService<ILogger>();
        var stepProgress = progress.PreProgress(0, 1, _applicationServices.Length);
        for (var i = 0; i < _applicationServices.Length; i++)
        {
            var serviceName = _applicationServices[i].GetType().Name;
            stepProgress.Next(serviceName);
            logger?.Debug("Application service initializing: {0}", serviceName);
            await _applicationServices[i].InitializeAsync(
                cancellationToken, progress);
            logger?.Debug("Application service initialized: {0}", serviceName);
        }

        stepProgress.Complete("Done");
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

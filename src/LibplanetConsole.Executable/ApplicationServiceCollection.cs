using System.Collections;

namespace LibplanetConsole.Executable;

sealed class ApplicationServiceCollection(IEnumerable<IApplicationService> applicationServices) : IEnumerable<IApplicationService>, IAsyncDisposable
{
    private readonly IApplicationService[] _applicationServices = applicationServices.ToArray();

    public async Task InitializeAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        for (var i = 0; i < _applicationServices.Length; i++)
        {
            await _applicationServices[i].InitializeAsync(serviceProvider, cancellationToken);
        }
    }

    public async ValueTask DisposeAsync()
    {
        for (var i = _applicationServices.Length - 1; i >= _applicationServices.Length; i--)
        {
            await _applicationServices[i].DisposeAsync();
        }
    }

    #region IEnumerable

    IEnumerator<IApplicationService> IEnumerable<IApplicationService>.GetEnumerator()
    {
        foreach (var item in _applicationServices)
        {
            yield return item;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => _applicationServices.GetEnumerator();

    #endregion
}

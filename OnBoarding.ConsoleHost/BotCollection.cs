using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using OnBoarding.ConsoleHost.Exceptions;

namespace OnBoarding.ConsoleHost;

[Export]
[Export(typeof(IApplicationService))]
[method: ImportingConstructor]
sealed class BotCollection(IServiceProvider serviceProvider) : IEnumerable<Bot>, IApplicationService
{
    private readonly OrderedDictionary _botByUser = [];
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public Bot AddNew(User user)
    {
        ArgumentExceptionUtility.ThrowIf(
            condition: _botByUser.Contains(user) == true,
            message: $"'{user}' is already included in the collection.",
            paramName: nameof(user));

        var serviceProvider = _serviceProvider;
        var bot = new Bot(serviceProvider, user);
        _botByUser.Add(user, bot);
        return bot;
    }

    public int Count => _botByUser.Count;

    public Bot this[int index] => (Bot)_botByUser[index]!;

    public Bot this[User user] => (Bot)_botByUser[user]!;

    public bool Contains(User user) => _botByUser.Contains(user);

    #region IApplicationService

    Task IApplicationService.InitializeAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken) => Task.CompletedTask;

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        foreach (var item in _botByUser.Values)
        {
            if (item is Bot { IsRunning: true } bot)
            {
                await bot.StopAsync(cancellationToken: default);
            }
        }
    }

    #endregion

    #region IEnumerable

    IEnumerator<Bot> IEnumerable<Bot>.GetEnumerator()
    {
        foreach (var item in _botByUser.Values)
        {
            if (item is Bot bot)
            {
                yield return bot;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => _botByUser.Values.GetEnumerator();

    #endregion
}

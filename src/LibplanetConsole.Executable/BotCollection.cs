// using System.Collections;
// using System.Collections.Specialized;
// using System.ComponentModel.Composition;
// using LibplanetConsole.Common.Exceptions;

// namespace LibplanetConsole.Executable;

// [Export]
// [Export(typeof(IApplicationService))]
// [method: ImportingConstructor]
// sealed class BotCollection(IServiceProvider serviceProvider)
// : IEnumerable<Bot>, IApplicationService
// {
//     private readonly OrderedDictionary _botByClient = [];
//     private readonly IServiceProvider _serviceProvider = serviceProvider;
//
//     public Bot AddNew(Client client)
//     {
//         ArgumentExceptionUtility.ThrowIf(
//             condition: _botByClient.Contains(client) == true,
//             message: $"'{client}' is already included in the collection.",
//             paramName: nameof(client));
//
//         var serviceProvider = _serviceProvider;
//         var bot = new Bot(serviceProvider, client);
//         _botByClient.Add(client, bot);
//         return bot;
//     }
//
//     public int Count => _botByClient.Count;
//
//     public Bot this[int index] => (Bot)_botByClient[index]!;
//
//     public Bot this[Client client] => (Bot)_botByClient[client]!;
//
//     public bool Contains(Client client) => _botByClient.Contains(client);
//
//     #region IApplicationService
//
//     Task IApplicationService.InitializeAsync(
//    IServiceProvider serviceProvider, CancellationToken cancellationToken) => Task.CompletedTask;
//
//     async ValueTask IAsyncDisposable.DisposeAsync()
//     {
//         foreach (var item in _botByClient.Values)
//         {
//             if (item is Bot { IsRunning: true } bot)
//             {
//                 await bot.StopAsync(cancellationToken: default);
//             }
//         }
//     }
//
//     #endregion
//
//     #region IEnumerable
//
//     IEnumerator<Bot> IEnumerable<Bot>.GetEnumerator()
//     {
//         foreach (var item in _botByClient.Values)
//         {
//             if (item is Bot bot)
//             {
//                 yield return bot;
//             }
//         }
//     }
//
//     IEnumerator IEnumerable.GetEnumerator() => _botByClient.Values.GetEnumerator();
//
//     #endregion
// }

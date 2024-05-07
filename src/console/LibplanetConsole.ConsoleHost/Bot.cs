// using LibplanetConsole.Common;
// using LibplanetConsole.Common.Exceptions;

// namespace LibplanetConsole.ConsoleHost;

// sealed class Bot(IServiceProvider serviceProvider, Client client)
// {
//     private Task? _task;
//     private CancellationTokenSource? _cancellationTokenSource;
//
//     public bool IsRunning { get; private set; }
//
//     public Client Client => client;
//
//     public async Task StartAsync(CancellationToken cancellationToken)
//     {
//         InvalidOperationExceptionUtility.ThrowIf(IsRunning == true, "Bot has been started.");
//
//         _cancellationTokenSource = new();
//         _task = RunAsync(_cancellationTokenSource.Token);
//         await Task.CompletedTask;
//         IsRunning = true;
//     }
//
//     public async Task StopAsync(CancellationToken cancellationToken)
//     {
//         InvalidOperationExceptionUtility.ThrowIf(IsRunning != true, "Bot has been stopped.");
//
//         _cancellationTokenSource!.Cancel();
//         _cancellationTokenSource = null;
//         await _task!;
//         _task = null;
//         IsRunning = false;
//     }
//
//     private async Task RunAsync(CancellationToken cancellationToken)
//     {
//         try
//         {
//             var random = new Random(client.GetHashCode());
//             var nodes = (NodeCollection)serviceProvider.GetService(typeof(NodeCollection))!;
//             while (cancellationToken.IsCancellationRequested == false)
//             {
//                 var v = random.Next(100);
//                 var tick = random.Next(100, 2000);
//                 await Task.Delay(tick, cancellationToken);
//                 var node = nodes.Current;
//                 if (client.IsOnline == false && v < 50)
//                 {
//                     client.Login(node);
//                 }
//                 else if (client.IsOnline == true && v < 10)
//                 {
//                     client.Logout();
//                 }
//                 else if (client.IsOnline == true && v < 50)
//                 {
//                     if (RandomUtility.GetNext(100) < 90)
//                     {
//                     }
//                     else if (client.PlayerInfo == null)
//                     {
//                         await client.CreateCharacterAsync(node, cancellationToken);
//                     }
//                     else if (client.PlayerInfo.Life <= 0)
//                     {
//                         await client.ReviveCharacterAsync(node, cancellationToken);
//                     }
//                     else
//                     {
//                         var @out = client.Out;
//                         var blockIndex = await client.PlayGameAsync(node, cancellationToken);
//                         client.Out = new StringWriter();
//                         await @out.WriteLineAsync("replaying.");
//                         await client.ReplayGameAsync(node, blockIndex, tick: 500,
// cancellationToken);
//                         client.Out = @out;
//                         await @out.WriteLineAsync("replayed.");
//                     }
//                 }
//
//             }
//         }
//         catch (TaskCanceledException)
//         {
//         }
//     }
// }

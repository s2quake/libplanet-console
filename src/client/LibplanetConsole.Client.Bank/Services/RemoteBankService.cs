// using LibplanetConsole.Bank;
// using LibplanetConsole.Bank.Actions;
// using LibplanetConsole.Bank.Services;
// using LibplanetConsole.Common.Services;

// namespace LibplanetConsole.Client.Bank.Services;

// internal sealed class RemoteBankService(IClient client)
//     : RemoteService<IBankService>, IBank
// {
//     public async Task<BalanceInfo> MintAsync(
//         decimal amount, CancellationToken cancellationToken)
//     {
//         var address = client.Address;
//         var actions = new IAction[]
//         {
//             new MintAction
//             {
//                 Address = address,
//                 Amount = amount,
//             },
//         };
//         await client.SendTransactionAsync(actions, cancellationToken);
//         return await Service.GetBalanceAsync(address, cancellationToken);
//     }

//     public async Task<BalanceInfo> TransferAsync(
//         decimal amount, Address targetAddress, CancellationToken cancellationToken)
//     {
//         var address = client.Address;
//         var actions = new IAction[]
//         {
//             new TransferAction
//             {
//                 TargetAddress = targetAddress,
//                 Amount = amount,
//             },
//         };
//         await client.SendTransactionAsync(actions, cancellationToken);
//         return await Service.GetBalanceAsync(address, cancellationToken);
//     }

//     public async Task<BalanceInfo> BurnAsync(
//         decimal amount, CancellationToken cancellationToken)
//     {
//         var address = client.Address;
//         var actions = new IAction[]
//         {
//             new BurnAction
//             {
//                 Address = address,
//                 Amount = amount,
//             },
//         };
//         await client.SendTransactionAsync(actions, cancellationToken);
//         return await Service.GetBalanceAsync(address, cancellationToken);
//     }

//     public Task<BalanceInfo> GetBalanceAsync(
//         Address address, CancellationToken cancellationToken)
//         => Service.GetBalanceAsync(address, cancellationToken);

//     public Task<PoolInfo> GetPoolAsync(CancellationToken cancellationToken)
//         => Service.GetPoolAsync(cancellationToken);
// }

// // File may only contain a single type
// #pragma warning disable SA1402
// using JSSoft.Communication;

// namespace LibplanetConsole.Common.Services;

// public class LocalService<TService, TCallback> : ILocalService
//     where TService : class
//     where TCallback : class
// {
//     private readonly ServerService<TService, TCallback> _serverService;

//     public LocalService(TService service)
//     {
//         _serverService = new ServerService<TService, TCallback>(service);
//     }

//     public LocalService()
//     {
//         var obj = this;
//         if (obj is TService service)
//         {
//             _serverService = new ServerService<TService, TCallback>(service);
//         }
//         else
//         {
//             throw new InvalidOperationException(
//                 $"'{GetType()}' must be implemented by '{typeof(TService)}'.");
//         }
//     }

//     IService ILocalService.Service => _serverService;

//     protected TCallback Callback => _serverService.Client;
// }

// public class LocalService<TService> : ILocalService
//     where TService : class
// {
//     private readonly ServerService<TService> _serverService;

//     public LocalService(TService service)
//     {
//         _serverService = new ServerService<TService>(service);
//     }

//     public LocalService()
//     {
//         var obj = this;
//         if (obj is TService service)
//         {
//             _serverService = new ServerService<TService>(service);
//         }
//         else
//         {
//             throw new InvalidOperationException(
//                 $"'{GetType()}' must be implemented by '{typeof(TService)}'.");
//         }
//     }

//     IService ILocalService.Service => _serverService;
// }

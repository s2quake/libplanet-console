// using System.ComponentModel.Composition;
// using LibplanetConsole.Common.Services;

// namespace LibplanetConsole.Consoles.Services;

// [Export]
// [method: ImportingConstructor]
// internal sealed class RemoteClientServiceContext(
//     [ImportMany] IEnumerable<IClientContentService> clientContentServices)
//     : RemoteServiceContext(GetRemoteServices(clientContentServices))
// {
//     private static IEnumerable<IRemoteService> GetRemoteServices(
//         IEnumerable<IClientContentService> clientContentServices)
//     {
//         return clientContentServices.Select(item => item.RemoteService);
//     }
// }
using LibplanetConsole.Common;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console;

public interface IConsole
{
    Task<TxId> SendTransactionAsync(IAction[] actions, CancellationToken cancellationToken);
}

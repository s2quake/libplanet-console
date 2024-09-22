using JSSoft.Commands;

namespace LibplanetConsole.Frameworks;

public interface IApplicationService
{
    Task InitializeAsync(CancellationToken cancellationToken, IProgress<ProgressInfo> progress);
}

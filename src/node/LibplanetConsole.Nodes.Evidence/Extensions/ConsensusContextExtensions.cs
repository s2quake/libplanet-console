// Reflection should not be used to increase accessibility of classes, methods, or fields
#pragma warning disable S3011
using System.Reflection;
using Libplanet.Net.Consensus;

namespace LibplanetConsole.Nodes.Evidence.Extensions;

internal static class ConsensusContextExtensions
{
    public static Context GetContext(this ConsensusContext @this)
    {
        var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
        var type = typeof(ConsensusContext);
        var propertyName = "CurrentContext";
        var propertyInfo = type.GetProperty(propertyName, bindingFlags) ??
            throw new InvalidOperationException($"{propertyName} property not found.");
        if (propertyInfo.GetValue(@this) is Context context)
        {
            return context;
        }

        throw new InvalidOperationException($"{propertyName} value cannot be null.");
    }

    public static async Task WaitUntilPreVoteAsync(
        this ConsensusContext @this, CancellationToken cancellationToken)
    {
        while (@this.Step != ConsensusStep.PreVote)
        {
            await Task.Delay(100, cancellationToken);
        }
    }
}

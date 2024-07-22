// Reflection should not be used to increase accessibility of classes, methods, or fields
#pragma warning disable S3011
using System.Reflection;
using Libplanet.Net.Consensus;

namespace LibplanetConsole.Nodes.Evidence.Extensions;

internal static class ConsensusReactorExtensions
{
    public static ConsensusContext GetConsensusContext(this ConsensusReactor @this)
    {
        var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
        var type = typeof(ConsensusReactor);
        var propertyName = "ConsensusContext";
        var propertyInfo = type.GetProperty(propertyName, bindingFlags) ??
            throw new InvalidOperationException($"{propertyName} property not found.");
        if (propertyInfo.GetValue(@this) is ConsensusContext consensusContext)
        {
            return consensusContext;
        }

        throw new InvalidOperationException($"{propertyName} value cannot be null.");
    }
}

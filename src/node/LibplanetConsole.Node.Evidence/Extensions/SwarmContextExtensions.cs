// Reflection should not be used to increase accessibility of classes, methods, or fields
#pragma warning disable S3011
using System.Reflection;
using Libplanet.Net;
using Libplanet.Net.Consensus;

namespace LibplanetConsole.Nodes.Evidence.Extensions;

internal static class SwarmContextExtensions
{
    public static ConsensusReactor GetConsensusReactor(this Swarm @this)
    {
        var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
        var propertyName = "ConsensusReactor";
        var propertyInfo = typeof(Swarm).GetProperty(propertyName, bindingFlags) ??
            throw new InvalidOperationException($"{propertyName} property not found.");
        if (propertyInfo.GetValue(@this) is ConsensusReactor consensusReactor)
        {
            return consensusReactor;
        }

        throw new InvalidOperationException($"{propertyName} value cannot be null.");
    }
}

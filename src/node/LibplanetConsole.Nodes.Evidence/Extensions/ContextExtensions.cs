// Reflection should not be used to increase accessibility of classes, methods, or fields
#pragma warning disable S3011
using System.Reflection;
using Libplanet.Net.Consensus;
using Libplanet.Net.Messages;
using Libplanet.Types.Consensus;

namespace LibplanetConsole.Nodes.Evidence.Extensions;

internal static class ContextExtensions
{
    public static ValidatorSet GetValidatorSet(this Context @this)
    {
        var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
        var type = typeof(Context);
        var fieldName = "_validatorSet";
        var fieldInfo = type.GetField(fieldName, bindingFlags) ??
            throw new InvalidOperationException($"{fieldName} field not found.");
        if (fieldInfo.GetValue(@this) is ValidatorSet validatorSet)
        {
            return validatorSet;
        }

        throw new InvalidOperationException($"{fieldName} value cannot be null.");
    }

    public static void PublishMessage(this Context @this, ConsensusMsg message)
    {
        var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
        var type = typeof(Context);
        var methodName = "PublishMessage";
        var methodInfo = type.GetMethod(methodName, bindingFlags) ??
            throw new InvalidOperationException($"{methodName} method not found.");

        var args = new object[] { message };
        methodInfo.Invoke(@this, args);
    }
}

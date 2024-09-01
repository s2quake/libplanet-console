#pragma warning disable SA1402 // File may only contain a single type
using System.ComponentModel.DataAnnotations;

namespace LibplanetConsole.Common.DataAnnotations;

[AttributeUsage(AttributeTargets.Property)]
public sealed class AppPeerAttribute : RegularExpressionAttribute
{
    public AppPeerAttribute()
        : base($"^{AppPeer.RegularExpression}$")
    {
    }
}

[AttributeUsage(AttributeTargets.Property)]
public sealed class AppPeerArrayAttribute : ArrayAttribute<AppPeerAttribute>
{
}

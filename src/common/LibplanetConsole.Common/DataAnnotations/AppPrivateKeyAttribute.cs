#pragma warning disable SA1402 // File may only contain a single type
using System.ComponentModel.DataAnnotations;

namespace LibplanetConsole.Common.DataAnnotations;

[AttributeUsage(AttributeTargets.Property)]
public sealed class AppPrivateKeyAttribute : RegularExpressionAttribute
{
    public AppPrivateKeyAttribute()
        : base($"^{AppPrivateKey.RegularExpression}$")
    {
    }
}

[AttributeUsage(AttributeTargets.Property)]
public sealed class AppPrivateKeyArrayAttribute : ArrayAttribute<AppPrivateKeyAttribute>
{
}

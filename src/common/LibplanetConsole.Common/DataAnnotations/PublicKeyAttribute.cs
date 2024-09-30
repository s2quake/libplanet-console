#pragma warning disable SA1402 // File may only contain a single type
using System.ComponentModel.DataAnnotations;
using LibplanetConsole.DataAnnotations;

namespace LibplanetConsole.Common.DataAnnotations;

[AttributeUsage(AttributeTargets.Property)]
public sealed class PublicKeyAttribute : RegularExpressionAttribute
{
    public const string RegularExpression = "(?:[0-9a-fA-F]{130}|[0-9a-fA-F]{66})";

    public PublicKeyAttribute()
        : base($"^{RegularExpression}$")
    {
    }
}

[AttributeUsage(AttributeTargets.Property)]
public sealed class PublicKeyArrayAttribute : ArrayAttribute<PublicKeyAttribute>
{
}

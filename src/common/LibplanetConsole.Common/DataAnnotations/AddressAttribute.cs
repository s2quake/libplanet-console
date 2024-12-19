#pragma warning disable SA1402 // File may only contain a single type
using System.ComponentModel.DataAnnotations;
using LibplanetConsole.DataAnnotations;

namespace LibplanetConsole.Common.DataAnnotations;

[AttributeUsage(AttributeTargets.Property)]
public sealed class AddressAttribute : RegularExpressionAttribute
{
    public const string RegularExpression = "(?:0x)?[0-9a-fA-F]{40}";

    public AddressAttribute()
        : base($"^{RegularExpression}$")
    {
    }
}

[AttributeUsage(AttributeTargets.Property)]
public sealed class AddressArrayAttribute : ArrayAttribute<AddressAttribute>
{
}

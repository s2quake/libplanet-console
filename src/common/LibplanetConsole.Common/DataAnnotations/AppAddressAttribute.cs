#pragma warning disable SA1402 // File may only contain a single type
using System.ComponentModel.DataAnnotations;
using LibplanetConsole.DataAnnotations;

namespace LibplanetConsole.Common.DataAnnotations;

[AttributeUsage(AttributeTargets.Property)]
public sealed class AppAddressAttribute : RegularExpressionAttribute
{
    public AppAddressAttribute()
        : base($"^{AppAddress.RegularExpression}$")
    {
    }
}

[AttributeUsage(AttributeTargets.Property)]
public sealed class AddressArrayAttribute : ArrayAttribute<AppAddressAttribute>
{
}

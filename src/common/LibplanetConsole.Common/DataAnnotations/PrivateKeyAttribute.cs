#pragma warning disable SA1402 // File may only contain a single type
using System.ComponentModel.DataAnnotations;
using LibplanetConsole.DataAnnotations;

namespace LibplanetConsole.Common.DataAnnotations;

[AttributeUsage(AttributeTargets.Property)]
public sealed class PrivateKeyAttribute : RegularExpressionAttribute
{
    public const string RegularExpression = "[0-9a-fA-F]{64}";

    public PrivateKeyAttribute()
        : base($"^{RegularExpression}$")
    {
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is PrivateKey)
        {
            return ValidationResult.Success;
        }

        return base.IsValid(value, validationContext);
    }
}

[AttributeUsage(AttributeTargets.Property)]
public sealed class PrivateKeyArrayAttribute : ArrayAttribute<PrivateKeyAttribute>
{
}

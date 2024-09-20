using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace LibplanetConsole.DataAnnotations;

[AttributeUsage(AttributeTargets.Property)]
public abstract class PropertyValidationAttribute : ValidationAttribute
{
    public Type? StaticType { get; set; }

    public string PropertyName { get; set; } = string.Empty;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (StaticType is not null && PropertyName == string.Empty)
        {
            return new ValidationResult(
                "PropertyName must not be empty, Because StaticType is not null.");
        }

        var propertyName = PropertyName;
        var type = StaticType ?? validationContext.ObjectType;
        var propertyInfo = type.GetRuntimeProperty(propertyName);
        if (propertyInfo is null)
        {
            return new ValidationResult(
                $"Property '{propertyName}' not found in '{type}'.");
        }

        if (propertyName != string.Empty)
        {
            value = propertyInfo.GetValue(validationContext.ObjectInstance);
        }

        if (IsValid(value) is false)
        {
            string[]? memberNames = validationContext.MemberName is { } memberName
                ? [memberName]
                : null;
            var errorMessage = FormatErrorMessage(validationContext.DisplayName);
            return new ValidationResult(errorMessage, memberNames);
        }

        return ValidationResult.Success;
    }
}

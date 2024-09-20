namespace LibplanetConsole.DataAnnotations;

public sealed class NotEqualToAttribute(object expectedValue) : PropertyValidationAttribute
{
    public override bool IsValid(object? value)
    {
        return Equals(value, expectedValue) is false;
    }
}

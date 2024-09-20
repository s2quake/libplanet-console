namespace LibplanetConsole.DataAnnotations;

public sealed class EqualToAttribute(object expectedValue) : PropertyValidationAttribute
{
    public override bool IsValid(object? value)
    {
        return Equals(value, expectedValue);
    }
}

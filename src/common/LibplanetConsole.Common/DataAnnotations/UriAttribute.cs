using System.ComponentModel.DataAnnotations;

namespace LibplanetConsole.Common.DataAnnotations;

[AttributeUsage(AttributeTargets.Property)]
public sealed class UriAttribute : DataTypeAttribute
{
    public UriAttribute()
        : base(DataType.Url)
    {
    }

    public bool AllowEmpty { get; set; }

    public override bool IsValid(object? value)
    {
        if (value is string @string)
        {
            if (AllowEmpty is true && @string == string.Empty)
            {
                return true;
            }

            if (Uri.TryCreate(@string, UriKind.Absolute, out _))
            {
                return true;
            }
        }

        return false;
    }
}

using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace LibplanetConsole.DataAnnotations;

[AttributeUsage(AttributeTargets.Property)]
public sealed class PositiveAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is sbyte and > 0
            or byte and > 0
            or short and > 0
            or ushort and > 0
            or int and > 0
            or uint and > 0
            or long and > 0
            or ulong and > 0
            or float and > 0
            or double and > 0
            or decimal and > 0)
        {
            return true;
        }

        if (value is BigInteger bigInteger && bigInteger > 0)
        {
            return true;
        }

        return false;
    }
}

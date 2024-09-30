#pragma warning disable SA1402 // File may only contain a single type
using System.ComponentModel.DataAnnotations;
using LibplanetConsole.DataAnnotations;

namespace LibplanetConsole.Common.DataAnnotations;

[AttributeUsage(AttributeTargets.Property)]
public sealed class EndPointAttribute : RegularExpressionAttribute
{
    public EndPointAttribute()
        : base($"^{AppEndPoint.RegularExpression}$")
    {
    }
}

[AttributeUsage(AttributeTargets.Property)]
public sealed class AppEndPointArrayAttribute : ArrayAttribute<EndPointAttribute>
{
}

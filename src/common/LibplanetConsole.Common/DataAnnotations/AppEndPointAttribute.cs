#pragma warning disable SA1402 // File may only contain a single type
using System.ComponentModel.DataAnnotations;

namespace LibplanetConsole.Common.DataAnnotations;

[AttributeUsage(AttributeTargets.Property)]
public sealed class AppEndPointAttribute : RegularExpressionAttribute
{
    public AppEndPointAttribute()
        : base($"^{AppEndPoint.RegularExpression}$")
    {
    }
}

[AttributeUsage(AttributeTargets.Property)]
public sealed class AppEndPointArrayAttribute : ArrayAttribute<AppEndPointAttribute>
{
}

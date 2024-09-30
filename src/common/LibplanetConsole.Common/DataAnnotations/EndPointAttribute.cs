#pragma warning disable SA1402 // File may only contain a single type
using System.ComponentModel.DataAnnotations;
using LibplanetConsole.DataAnnotations;

namespace LibplanetConsole.Common.DataAnnotations;

[AttributeUsage(AttributeTargets.Property)]
public sealed class EndPointAttribute : RegularExpressionAttribute
{
    public const string HostExpression
        = @"(?:(?:[a-zA-Z0-9\-\.]+)|(?:\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}))";

    public const string PortExpression = @"\d{1,5}";
    public static readonly string RegularExpression
        = $"{HostExpression}:{PortExpression}";

    public EndPointAttribute()
        : base($"^{RegularExpression}$")
    {
    }
}

[AttributeUsage(AttributeTargets.Property)]
public sealed class EndPointArrayAttribute : ArrayAttribute<EndPointAttribute>
{
}

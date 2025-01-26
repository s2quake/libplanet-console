using System.ComponentModel;
using System.Globalization;
using Libplanet.Types.Evidence;

namespace LibplanetConsole.Common.Converters;

public sealed class EvidenceIdTypeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

    public override object? ConvertFrom(
        ITypeDescriptorContext? context, CultureInfo? culture, object value)
        => value is string v ? EvidenceId.Parse(v) : base.ConvertFrom(context, culture, value);

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
        => destinationType == typeof(string) || base.CanConvertTo(context, destinationType);

    public override object? ConvertTo(
        ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        => value is EvidenceBase evidence && destinationType == typeof(string)
            ? evidence.ToString()
            : base.ConvertTo(context, culture, value, destinationType);
}

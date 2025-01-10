using System.ComponentModel;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;

#if LIBPLANET_NODE
namespace LibplanetConsole.Node.Converters;
#elif LIBPLANET_CLIENT
namespace LibplanetConsole.Client.Converters;
#elif LIBPLANET_CONSOLE
namespace LibplanetConsole.Console.Converters;
#else
#error LIBPLANET_NODE, LIBPLANET_CLIENT, or LIBPLANET_CONSOLE must be defined.
#endif

internal sealed class AddressTypeConverter : TypeConverter
{
    private static TypeConverter? _originalConverter;

    public static void Register()
    {
        if (_originalConverter is not null)
        {
            throw new InvalidOperationException("Already registered.");
        }

        _originalConverter = TypeDescriptor.GetConverter(typeof(Address));
        TypeDescriptor.AddAttributes(
            typeof(Address), new TypeConverterAttribute(typeof(AddressTypeConverter)));
    }

    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        if (_originalConverter is null)
        {
            throw new InvalidOperationException("Not registered.");
        }

        return _originalConverter.CanConvertFrom(context, sourceType);
    }

    public override object? ConvertFrom(
        ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (_originalConverter is null)
        {
            throw new InvalidOperationException("Not registered.");
        }

        if (value is string text
            && context?.GetRequiredService<IAddressCollection>() is { } addresses)
        {
            return addresses.ToAddress(text);
        }

        return _originalConverter.ConvertFrom(context, culture, value);
    }

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
        if (_originalConverter is null)
        {
            throw new InvalidOperationException("Not registered.");
        }

        return _originalConverter.CanConvertTo(context, destinationType);
    }

    public override object? ConvertTo(
        ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (_originalConverter is null)
        {
            throw new InvalidOperationException("Not registered.");
        }

        return _originalConverter.ConvertTo(context, culture, value, destinationType);
    }
}

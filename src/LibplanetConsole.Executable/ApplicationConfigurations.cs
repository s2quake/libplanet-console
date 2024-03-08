using System.Collections.Immutable;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Reflection;
using JSSoft.Library;

namespace LibplanetConsole.Executable;

[Export]
sealed class ApplicationConfigurations : Configurations
{
    private readonly ImmutableDictionary<Type, IApplicationConfiguration> _configurationByType;
    private readonly ImmutableDictionary<string, ConfigurationDescriptorBase> _descriptorByKey;

    [ImportingConstructor]
    public ApplicationConfigurations([ImportMany] IEnumerable<IApplicationConfiguration> configurations)
        : base(configurations.Select(item => item.GetType()))
    {
        _configurationByType = configurations.ToImmutableDictionary(item => item.GetType());
        _descriptorByKey = Descriptors.ToImmutableDictionary(item => $"{item}");
    }

    public string? GetValue(string key)
    {
        var descriptor = _descriptorByKey[key];
        var propertyInfo = (PropertyInfo)descriptor.Owner;
        var instance = _configurationByType[propertyInfo.DeclaringType!];
        var converter = TypeDescriptor.GetConverter(propertyInfo.PropertyType);
        var value = propertyInfo.GetValue(instance);
        return converter.ConvertToString(value);
    }

    public void SetValue(string key, string value)
    {
        var descriptor = _descriptorByKey[key];
        var propertyInfo = (PropertyInfo)descriptor.Owner;
        var instance = _configurationByType[propertyInfo.DeclaringType!];
        var converter = TypeDescriptor.GetConverter(propertyInfo.PropertyType);
        var v = converter.ConvertFromString(value);
        propertyInfo.SetValue(instance, v);
    }
}

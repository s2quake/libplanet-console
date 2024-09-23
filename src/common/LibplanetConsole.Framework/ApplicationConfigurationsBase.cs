using JSSoft.Configurations;
using JSSoft.Configurations.Extensions;

namespace LibplanetConsole.Framework;

public abstract class ApplicationConfigurationsBase : ConfigurationsBase
{
    private readonly IApplicationConfiguration[] _configurations;

    protected ApplicationConfigurationsBase(IEnumerable<IApplicationConfiguration> configurations)
        : base(configurations.Select(item => item.GetType()))
    {
        _configurations = [.. configurations];
        Commit(_configurations);
    }

    public new string this[string key]
    {
        get => ConfigurationsBaseExtensions.GetValue(this, key);
        set
        {
            ConfigurationsBaseExtensions.SetValue(this, key, value);
            Update(_configurations);
        }
    }

    public IEnumerator<string> GetEnumerator() => Keys.GetEnumerator();
}

using System.Text.Json.Serialization;

namespace LibplanetConsole.Options;

public abstract class OptionsBase<T>
    where T : OptionsBase<T>
{
    [JsonIgnore]
    public bool IsConfigured { get; internal set; }

    [JsonIgnore]
    public bool IsValidated { get; internal set; }

    public T Verify()
    {
        if (!IsConfigured)
        {
            throw new InvalidOperationException(
                $"The options {typeof(T)} have not been configured yet.");
        }

        if (!IsValidated)
        {
            throw new InvalidOperationException(
                $"The options {typeof(T)} have not been validated yet.");
        }

        return (T)this;
    }
}

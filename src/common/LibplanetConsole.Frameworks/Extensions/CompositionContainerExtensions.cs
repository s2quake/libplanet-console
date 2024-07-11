using System.ComponentModel.Composition.Hosting;

namespace LibplanetConsole.Frameworks.Extensions;

public static class CompositionContainerExtensions
{
    public static T GetValue<T>(this CompositionContainer @this)
        => @this.GetExportedValue<T>() ??
            throw new InvalidOperationException($"'{typeof(T)}' is not found.");
}

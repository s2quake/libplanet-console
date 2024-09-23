using System.ComponentModel.Composition.Primitives;

namespace LibplanetConsole.Frameworks;

internal sealed class ApplicationExport(Export export, Action<IAsyncDisposable> exportAction)
    : Export
{
    public override ExportDefinition Definition => export.Definition;

    protected override object? GetExportedValueCore()
    {
        if (export.Value is IAsyncDisposable asyncDisposable)
        {
            exportAction.Invoke(asyncDisposable);
        }

        return export.Value;
    }
}

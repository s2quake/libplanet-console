// // // using System.ComponentModel.Composition.Primitives;

namespace LibplanetConsole.Framework.Extensions;

public static class CompositionContainerExtensions
{
    // public static T GetValue<T>(this CompositionContainer @this)
    //     => @this.GetExportedValue<T>() ??
    //         throw new InvalidOperationException($"'{typeof(T)}' is not found.");

    // public static void ComposeExportedValues(this CompositionContainer @this, object[] components)
    // {
    //     var batch = new CompositionBatch();
    //     foreach (var item in components)
    //     {
    //         var contractName = AttributedModelServices.GetContractName(item.GetType());
    //         var typeIdentity = AttributedModelServices.GetTypeIdentity(item.GetType());
    //         var metadata = new Dictionary<string, object?>
    //         {
    //             { "ExportTypeIdentity", typeIdentity },
    //         };
    //         var export = new Export(contractName, metadata, () => item);
    //         batch.AddExport(export);
    //     }

    //     @this.Compose(batch);
    // }
}

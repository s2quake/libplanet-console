// using System.Data;
// using System.Reflection;
// using Microsoft.Extensions.DependencyInjection;

// namespace LibplanetConsole.Framework;

// public sealed class ApplicationContainer : ServiceCollection, IAsyncDisposable
// {
//     private readonly object _owner;
//     private readonly ApplicationContainer? _parentContainer;

//     public ApplicationContainer(object owner)
//     {
//         _owner = owner;
//     }

//     public ApplicationContainer(
//         object owner, ApplicationContainer container)
//     {
//         _parentContainer = container;
//         _owner = owner;
//     }

//     public static IEnumerable<Assembly> GetAssemblies(object owner)
//         => GetAssemblies(owner.GetType().Assembly);

//     public static IEnumerable<Assembly> GetAssemblies()
//         => GetAssemblies(Assembly.GetEntryAssembly()!);

//     public static IEnumerable<Assembly> GetAssemblies(Assembly assembly)
//     {
//         var directory = Path.GetDirectoryName(assembly.Location)!;
//         var files = Directory.GetFiles(directory, "LibplanetConsole.*.dll");
//         string[] paths =
//         [
//             assembly.Location,
//             .. files,
//         ];
//         return [.. paths.Distinct().Order().Select(Assembly.LoadFrom)];
//     }
// }

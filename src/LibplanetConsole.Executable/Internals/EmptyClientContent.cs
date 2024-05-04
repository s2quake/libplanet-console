using System.ComponentModel.Composition;

namespace LibplanetConsole.Executable.Internals;

[Export(typeof(IClientContent))]
[method: ImportingConstructor]
internal sealed class EmptyClientContent(IClient client) : ClientContentBase(client)
{
}

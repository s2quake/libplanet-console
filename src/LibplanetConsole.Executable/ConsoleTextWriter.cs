using System.Text;

namespace LibplanetConsole.Executable;

internal sealed class ConsoleTextWriter : TextWriter
{
    public override Encoding Encoding => Encoding.UTF8;

    internal static SynchronizationContext? SynchronizationContext { get; set; }

    public override void Write(char value) => Console.Write(value);

    public override void Write(string? value) => Console.Write(value);

    public override void WriteLine(string? value) => Console.WriteLine(value);

    public override Task WriteAsync(char value)
    {
        return Task.Run(() => SynchronizationContext!.Send((s) => Console.Write(value), null));
    }

    public override Task WriteAsync(string? value)
    {
        return Task.Run(() => SynchronizationContext!.Send((s) => Console.Write(value), null));
    }

    public override Task WriteLineAsync(string? value)
    {
        return Task.Run(() => SynchronizationContext!.Send((s) => Console.WriteLine(value), null));
    }
}

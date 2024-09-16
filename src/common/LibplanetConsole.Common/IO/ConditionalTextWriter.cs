using System.Text;

namespace LibplanetConsole.Common.IO;

public sealed class ConditionalTextWriter(TextWriter writer) : TextWriter
{
    public bool Condition { get; set; }

    public override Encoding Encoding { get; } = writer.Encoding;

    public override void Write(char value)
    {
        if (Condition is true)
        {
            writer.Write(value);
        }
    }

    public override void Write(string? value)
    {
        if (Condition is true)
        {
            writer.Write(value);
        }
    }

    public override void WriteLine(string? value)
    {
        if (Condition is true)
        {
            writer.WriteLine(value);
        }
    }

    public override Task WriteAsync(char value)
    {
        if (Condition is true)
        {
            return writer.WriteAsync(value);
        }

        return Task.CompletedTask;
    }

    public override Task WriteAsync(string? value)
    {
        if (Condition is true)
        {
            return writer.WriteAsync(value);
        }

        return Task.CompletedTask;
    }

    public override Task WriteLineAsync(string? value)
    {
        if (Condition is true)
        {
            return writer.WriteLineAsync(value);
        }

        return Task.CompletedTask;
    }
}

namespace LibplanetConsole.Executable.Extensions;

static class TextWriterExtensions
{
    public static void WriteLineAsJson(this TextWriter @this, object obj)
    {
        var json = JsonUtility.SerializeObject(obj, isColorized: true);
        @this.WriteLine(json);
    }

    public static Task WriteLineAsJsonAsync(this TextWriter @this, object obj)
    {
        var json = JsonUtility.SerializeObject(obj, isColorized: true);
        return @this.WriteLineAsync(json);
    }
}

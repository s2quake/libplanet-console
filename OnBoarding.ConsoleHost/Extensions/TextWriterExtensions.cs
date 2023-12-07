namespace OnBoarding.ConsoleHost.Extensions;

static class TextWriterExtensions
{
    public static void WriteLineAsJson(this TextWriter @this, object obj)
    {
        var json = JsonUtility.SerializeObject(obj, isColorized: true);
        @this.WriteLine(json);
    }
}

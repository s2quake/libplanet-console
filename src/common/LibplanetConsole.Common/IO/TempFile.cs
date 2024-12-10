namespace LibplanetConsole.Common.IO;

public sealed class TempFile : IDisposable
{
    private static readonly string _directory = Path.Combine(
        Path.GetTempPath(),
        AppDomain.CurrentDomain.FriendlyName,
        $"{Environment.ProcessId}");

    static TempFile()
    {
        DirectoryUtility.EnsureDirectory(_directory);
        AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

        static void CurrentDomain_ProcessExit(object? sender, EventArgs e)
            => DirectoryUtility.TryDelete(_directory, recursive: true);
    }

    public TempFile()
    {
    }

    private TempFile(string content) => File.WriteAllText(FileName, content);

    private TempFile(byte[] bytes) => File.WriteAllBytes(FileName, bytes);

    public string FileName { get; } = Path.Combine(_directory, Path.GetRandomFileName());

    public static implicit operator string(TempFile tempFile) => tempFile.FileName;

    public static TempFile WriteAllText(string content) => new(content);

    public static TempFile WriteAllBytes(byte[] bytes) => new(bytes);

    public void Dispose() => FileUtility.TryDelete(FileName);

    public override string ToString() => FileName;
}

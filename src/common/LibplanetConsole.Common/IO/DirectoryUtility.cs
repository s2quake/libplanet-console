namespace LibplanetConsole.Common.IO;

public static class DirectoryUtility
{
    public static string EnsureDirectory(string path)
    {
        if (Directory.Exists(path) is false)
        {
            return Directory.CreateDirectory(path).FullName;
        }

        return Path.GetFullPath(path);
    }

    public static void DeleteIfExists(string path)
    {
        if (Directory.Exists(path) == true)
        {
            Directory.Delete(path);
        }
    }

    public static bool TryDelete(string path, bool recursive)
    {
        try
        {
            if (Directory.Exists(path) == true)
            {
                Directory.Delete(path, recursive);
                return true;
            }
        }
        catch
        {
            // ignored
        }

        return false;
    }
}

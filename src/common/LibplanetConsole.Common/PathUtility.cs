namespace LibplanetConsole.Common;

public static class PathUtility
{
    public static string GetRelativePath(string fromFile, string toFile)
    {
        if (Path.IsPathRooted(fromFile) is false)
        {
            throw new ArgumentException(
                $"The {nameof(fromFile)} must be an absolute path.", nameof(fromFile));
        }

        if (Path.IsPathRooted(toFile) is false)
        {
            throw new ArgumentException(
                $"The {nameof(toFile)} must be an absolute path.", nameof(toFile));
        }

        var fromUri = new Uri(fromFile);
        var toUri = new Uri(toFile);

        if (fromUri.Scheme != toUri.Scheme)
        {
            return toFile;
        }

        var relativeUri = fromUri.MakeRelativeUri(toUri);
        return Uri.UnescapeDataString(relativeUri.ToString());
    }

    public static string GetRelativePathFromDirectory(string fromDirectory, string toFile)
    {
        if (Path.IsPathRooted(fromDirectory) is false)
        {
            throw new ArgumentException(
                $"The {nameof(fromDirectory)} must be an absolute path.", nameof(fromDirectory));
        }

        if (Path.IsPathRooted(toFile) is false)
        {
            throw new ArgumentException(
                $"The {nameof(toFile)} must be an absolute path.", nameof(toFile));
        }

        var fromUri = new Uri(Path.Combine(fromDirectory, "."));
        var toUri = new Uri(toFile);

        if (fromUri.Scheme != toUri.Scheme)
        {
            return toFile;
        }

        var relativeUri = fromUri.MakeRelativeUri(toUri);
        return Uri.UnescapeDataString(relativeUri.ToString());
    }

    public static void EnsureDirectory(string directoryPath)
    {
        if (Path.IsPathRooted(directoryPath) is false)
        {
            throw new ArgumentException(
                $"'{nameof(directoryPath)}' must be an absolute path.", nameof(directoryPath));
        }

        if (Directory.Exists(directoryPath) is false)
        {
            Directory.CreateDirectory(directoryPath);
        }
    }

    public static void EnsureDirectoryForFile(string filePath)
    {
        if (Path.IsPathRooted(filePath) is false)
        {
            throw new ArgumentException(
                $"'{nameof(filePath)}' must be an absolute path.", nameof(filePath));
        }

        var directoryName = Path.GetDirectoryName(filePath)
            ?? throw new ArgumentException("The file path is invalid.", nameof(filePath));

        if (Directory.Exists(directoryName) is false)
        {
            Directory.CreateDirectory(directoryName);
        }
    }
}

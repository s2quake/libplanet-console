namespace LibplanetConsole.Consoles;

internal static class ProcessUtility
{
    private const string WorkspacePathVariableName = "LIBPLANET_CONSOLE_WORKSPACE_PATH";
    private const string NodePathVariableName = "LIBPLANET_CONSOLE_NODE_PATH";
    private const string ClientPathVariableName = "LIBPLANET_CONSOLE_CLIENT_PATH";
    private const string Framework = "net8.0";

#if DEBUG
    private const string Congiguration = "Debug";
#elif RELEASE
    private const string Congiguration = "Release";
#endif

    public static string WorkspacePath
    {
        get
        {
            if (Environment.GetEnvironmentVariable(WorkspacePathVariableName) is { } workspacePath)
            {
                if (Directory.Exists(workspacePath) != true)
                {
                    var message =
                        $"Directory '{workspacePath}' of environment variable " +
                        $"'{WorkspacePathVariableName}' does not exist.";
                    throw new InvalidOperationException(message);
                }

                return workspacePath;
            }
            else
            {
                throw new InvalidOperationException(
                    $"Environment variable '{WorkspacePathVariableName}' is not set.");
            }
        }
    }

    public static string NodePath
    {
        get
        {
            if (Environment.GetEnvironmentVariable(NodePathVariableName) is { } nodePath)
            {
                if (File.Exists(nodePath) != true)
                {
                    var message =
                        $"File '{nodePath}' of environment variable " +
                        $"'{NodePathVariableName}' does not exist.";
                    throw new FileNotFoundException(message);
                }

                return nodePath;
            }

            var paths = new string[]
            {
                WorkspacePath,
                $"src/node/LibplanetConsole.NodeHost/bin/{Congiguration}/" +
                $"{Framework}/libplanet-node{Extension}",
            };
            nodePath = Path.GetFullPath(Path.Combine(paths));

            if (File.Exists(nodePath) != true)
            {
                var message = $"File '{nodePath}' does not exist.";
                throw new FileNotFoundException(message);
            }

            return nodePath;
        }
    }

    public static string ClientPath
    {
        get
        {
            if (Environment.GetEnvironmentVariable(ClientPathVariableName) is { } clientPath)
            {
                if (File.Exists(clientPath) != true)
                {
                    var message =
                        $"File '{clientPath}' of environment variable " +
                        $"'{ClientPathVariableName}' does not exist.";
                    throw new FileNotFoundException(message);
                }

                return clientPath;
            }

            var paths = new string[]
            {
                WorkspacePath,
                $"src/client/LibplanetConsole.ClientHost/bin/{Congiguration}/" +
                $"{Framework}/libplanet-client{Extension}",
            };
            clientPath = Path.GetFullPath(Path.Combine(paths));

            if (File.Exists(clientPath) != true)
            {
                var message = $"File '{clientPath}' does not exist.";
                throw new FileNotFoundException(message);
            }

            return clientPath;
        }
    }

    public static string Extension => IsWindows() ? ".exe" : ".dll";

    public static string GetNodePath()
    {
        try
        {
            return NodePath;
        }
        catch (Exception e)
        {
            var message =
                $"Use 'src/node/LibplanetConsole.NodeHost/bin/{Congiguration}/{Framework}/" +
                $"libplanet-node{Extension}' by setting the directory path " +
                $"in environment variable '{WorkspacePathVariableName}', or " +
                $"set the path to the node executable DLL file directly in environment variable " +
                $"'{NodePathVariableName}'.";
            throw new InvalidOperationException(message, innerException: e);
        }
    }

    public static string GetClientPath()
    {
        try
        {
            return ClientPath;
        }
        catch (Exception e)
        {
            var message =
                $"Use 'src/client/LibplanetConsole.ClientHost/bin/{Congiguration}/" +
                $"{Framework}/libplanet-client{Extension}' by setting the directory path " +
                $"in environment variable '{WorkspacePathVariableName}', or " +
                $"set the path to the node executable DLL file directly in environment variable " +
                $"'{ClientPathVariableName}'.";
            throw new InvalidOperationException(message, innerException: e);
        }
    }

    public static bool IsWindows()
    {
#if !NETSTANDARD && !NETFRAMEWORK && !NETCOREAPP
        return OperatingSystem.IsWindows();
#else
        return System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
            System.Runtime.InteropServices.OSPlatform.Windows);
#endif
    }
}
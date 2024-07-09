namespace LibplanetConsole.Databases;

public sealed record class DatabaseOptions
{
    public required string DatabasePath { get; init; } = string.Empty;

    public void Verify()
    {
        if (DatabasePath == string.Empty)
        {
            throw new InvalidOperationException($"{nameof(DatabasePath)} cannot be empty.");
        }

        if (File.Exists(DatabasePath) == true)
        {
            throw new InvalidOperationException($"{DatabasePath} already exists as a file.");
        }
    }
}

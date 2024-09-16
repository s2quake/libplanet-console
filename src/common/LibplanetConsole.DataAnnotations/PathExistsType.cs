namespace LibplanetConsole.DataAnnotations;

public enum PathExistsType
{
    /// <summary>
    /// No path existence check is performed.
    /// </summary>
    None,

    /// <summary>
    /// The specified path must not exist.
    /// </summary>
    NotExist,

    /// <summary>
    /// The specified path must either not exist or be empty.
    /// </summary>
    NotExistOrEmpty,

    /// <summary>
    /// The specified path must exist.
    /// </summary>
    Exist,
}

namespace LibplanetConsole.Frameworks;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DependencyAttribute : Attribute
{
    private Type? _type;

    public DependencyAttribute(string typeName) => DependencyTypeName = typeName;

    public DependencyAttribute(Type type)
    {
        _type = type;
        DependencyTypeName = type.AssemblyQualifiedName ?? string.Empty;
    }

    public string DependencyTypeName { get; }

    internal Type DependencyType
    {
        get
        {
            _type ??= Type.GetType(DependencyTypeName);
            if (_type is null)
            {
                throw new InvalidOperationException($"Type '{DependencyTypeName}' not found.");
            }

            return _type;
        }
    }
}

namespace LibplanetConsole.Common;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class ExportAttribute : Attribute
{
    public ExportAttribute()
    {
    }

    public ExportAttribute(Type type)
    {
    }
}

using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using CommunicationUtility = JSSoft.Communication.EndPointUtility;

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

using JSSoft.Commands;
using Libplanet.Net;
using LibplanetConsole.Common;
using LibplanetConsole.Common.DataAnnotations;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Node.Executable.EntryCommands;

[CommandSummary("Create AppProtocolVersion")]
internal sealed class AppProtocolVersionCommand : CommandBase
{
    private static readonly Codec _codec = new();

    public AppProtocolVersionCommand()
        : base("apv")
    {
    }

    [CommandPropertyRequired(DefaultValue = "")]
    [CommandSummary("The private key of the signer. If omitted, a random private key is used")]
    [PrivateKey]
    public string PrivateKey { get; set; } = string.Empty;

    [CommandPropertyRequired(DefaultValue = 1)]
    [CommandSummary("The version number. Default is 1")]
    public int Version { get; set; }

    [CommandProperty]
    [CommandSummary("The extra data to be included in the AppProtocolVersion")]
    public string Extra { get; set; } = string.Empty;

    [CommandPropertySwitch("raw")]
    [CommandSummary("If set, only the AppProtocolVersion is displayed")]
    public bool IsRaw { get; set; }

    protected override void OnExecute()
    {
        var signer = PrivateKeyUtility.ParseOrRandom(PrivateKey);
        var version = Version;
        var extra = GetExtraValue(Extra);
        var apv = AppProtocolVersion.Sign(signer, version, extra);
        if (IsRaw is true)
        {
            Out.WriteLine(apv.Token);
        }
        else
        {
            Out.WriteLineAsJson(new
            {
                PrivateKey = PrivateKeyUtility.ToString(signer),
                Version,
                Extra,
                AppProtocolVersion = apv.Token,
            });
        }
    }

    private static IValue? GetExtraValue(string extra)
    {
        if (extra == string.Empty)
        {
            return null;
        }

        return _codec.Decode(ByteUtil.ParseHex(extra));
    }
}

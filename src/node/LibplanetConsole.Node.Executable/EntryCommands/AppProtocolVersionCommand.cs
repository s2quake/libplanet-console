using JSSoft.Commands;
using Libplanet.Net;
using LibplanetConsole.Common;
using LibplanetConsole.Common.DataAnnotations;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Node.Executable.EntryCommands;

[CommandSummary("Creates AppProtocolVersion")]
internal sealed class AppProtocolVersionCommand : CommandBase
{
    private static readonly Codec _codec = new();

    public AppProtocolVersionCommand()
        : base("apv")
    {
    }

    [CommandPropertyRequired(DefaultValue = "")]
    [CommandSummary("Specifies the private key of the signer")]
    [PrivateKey]
    public string PrivateKey { get; set; } = string.Empty;

    [CommandPropertyRequired(DefaultValue = 1)]
    [CommandSummary("Specifies the version number. Default is 1")]
    public int Version { get; set; }

    [CommandProperty]
    [CommandSummary("Specifies the extra data to be included in the AppProtocolVersion")]
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

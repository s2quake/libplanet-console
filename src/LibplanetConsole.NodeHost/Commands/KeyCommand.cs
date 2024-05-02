using System.ComponentModel.Composition;
using JSSoft.Commands;
using Libplanet.Crypto;
using LibplanetConsole.Common;

namespace LibplanetConsole.NodeHost.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Provides Key-related commands.")]
internal sealed class KeyCommand : CommandMethodBase
{
    [CommandPropertySwitch('p')]
    public bool IsPrivateKey { get; set; }

    [CommandMethod]
    public void New()
    {
        var privateKey = new PrivateKey();
        Out.WriteLine(PrivateKeyUtility.ToString(privateKey));
    }

    [CommandMethod]
    public void Public(string privateKey)
    {
        var publicKey = PrivateKeyUtility.Parse(privateKey).PublicKey;
        Out.WriteLine(PublicKeyUtility.ToString(publicKey));
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(IsPrivateKey))]
    public void Address(string publicKey)
    {
        if (IsPrivateKey == true)
        {
            Out.WriteLine(PrivateKeyUtility.Parse(publicKey).Address);
        }
        else
        {
            Out.WriteLine(PublicKeyUtility.Parse(publicKey).Address);
        }
    }

    [CommandMethod]
    public void Derive(string address, string name)
    {
        Out.WriteLine(AddressUtility.Parse(address).de);
    }
}

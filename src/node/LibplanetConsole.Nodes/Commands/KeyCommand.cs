using System.ComponentModel.Composition;
using JSSoft.Commands;
using Libplanet.Crypto;
using LibplanetConsole.Common;

namespace LibplanetConsole.Nodes.Commands;

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
        var privateKeyObject = PrivateKeyUtility.Parse(privateKey);
        Out.WriteLine(PublicKeyUtility.ToString(privateKeyObject.PublicKey));
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
    public void Derive(string address, string key)
    {
        var addressObject = AddressUtility.Parse(address);
        Out.WriteLine(AddressUtility.Derive(addressObject, key));
    }
}

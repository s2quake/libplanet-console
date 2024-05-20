using System.ComponentModel.Composition;
using JSSoft.Commands;
using Libplanet.Crypto;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Consoles.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Provides Key-related commands.")]
internal sealed class KeyCommand : CommandMethodBase
{
    [CommandMethod]
    public void New()
    {
        var privateKey = new PrivateKey();
        var info = new
        {
            PrivateKey = PrivateKeyUtility.ToString(privateKey),
            PublicKey = PublicKeyUtility.ToString(privateKey.PublicKey),
            Address = AddressUtility.ToString(privateKey.Address),
        };
        Out.WriteLineAsJson(info);
    }

    [CommandMethod]
    public void Public(string privateKey)
    {
        var key = PrivateKeyUtility.Parse(privateKey);
        var info = new
        {
            PublicKey = PublicKeyUtility.ToString(key.PublicKey),
        };
        Out.WriteLineAsJson(info);
    }

    [CommandMethod]
    public void Address(
        [CommandSummary("The private key or public key.")]
        string key)
    {
        var address = GetAddress();
        var info = new
        {
            Address = AddressUtility.ToString(address),
        };
        Out.WriteLineAsJson(info);

        Address GetAddress()
        {
            if (PrivateKeyUtility.TryParse(key, out var privateKey) == true)
            {
                return privateKey.Address;
            }
            else if (PublicKeyUtility.TryParse(key, out var publicKey) == true)
            {
                return publicKey.Address;
            }
            else
            {
                throw new ArgumentException("Invalid key.", nameof(key));
            }
        }
    }

    [CommandMethod]
    public void Derive(string address, string key)
    {
        var addressObject = AddressUtility.Parse(address);
        var info = new
        {
            Address = AddressUtility.ToString(AddressUtility.Derive(addressObject, key)),
        };
        Out.WriteLineAsJson(info);
    }
}

using JSSoft.Commands;
using Libplanet.Crypto;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Common.Commands;

[CommandSummary("Provides Key-related commands.")]
public abstract class KeyCommandBase : CommandMethodBase
{
    [CommandMethod]
    [CommandSummary("Generates a new private key.")]
    public void New(
        [CommandSummary("The number of private keys to generate.")]
        int count = 1)
    {
        if (count < 1)
        {
            var message = "Count must be greater than or equal to 1.";
            throw new ArgumentOutOfRangeException(nameof(count), message);
        }

        var info = Enumerable.Range(0, count).Select(_ =>
        {
            var privateKey = new AppPrivateKey();
            return new
            {
                PrivateKey = AppPrivateKey.ToString(privateKey),
                privateKey.PublicKey,
                PublicKeyShort = privateKey.PublicKey.ToShortString(),
                privateKey.Address,
            };
        }).ToArray();
        Out.WriteLineAsJson(info);
    }

    [CommandMethod]
    [CommandSummary("Displays the public key from the given private key.")]
    public void Public(
        [CommandSummary("Indicates the private key that corresponds to the public key.")]
        string privateKey)
    {
        var key = AppPrivateKey.Parse(privateKey);
        var info = new
        {
            key.PublicKey,
            PublicKeyShort = key.PublicKey.ToShortString(),
        };
        Out.WriteLineAsJson(info);
    }

    [CommandMethod]
    [CommandSummary("Displays the public key from the given private or public key")]
    public void Address(
        [CommandSummary("Indicates the private or public key that corresponds to the address.")]
        string key)
    {
        var info = new
        {
            Address = GetAddress(key),
        };
        Out.WriteLineAsJson(info);
    }

    [CommandMethod]
    [CommandSummary("Derives a new address from the given address and keyword.")]
    public void Derive(
        [CommandSummary("The address.")]
        string address,
        [CommandSummary("The keyword.")]
        string keyword)
    {
        var info = new
        {
            Address = new Address(address).Derive(keyword),
        };
        Out.WriteLineAsJson(info);
    }

    private static Address GetAddress(string key)
    {
        if (AppPrivateKey.TryParse(key, out var privateKey) == true)
        {
            return privateKey.Address;
        }

        if (AppPublicKey.TryParse(key, out var publicKey) == true)
        {
            return publicKey.Address;
        }

        throw new ArgumentException("Invalid key.", nameof(key));
    }
}

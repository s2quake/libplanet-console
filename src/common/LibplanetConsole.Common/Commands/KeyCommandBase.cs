using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Common.Commands;

[CommandSummary("Provides key-related commands")]
public abstract class KeyCommandBase : CommandMethodBase
{
    [CommandMethod]
    [CommandSummary("Generates a new private key")]
    public void New(
        [CommandSummary("Specifies the number of private keys to generate")]
        int count = 1)
    {
        if (count < 1)
        {
            var message = "Count must be greater than or equal to 1.";
            throw new ArgumentOutOfRangeException(nameof(count), message);
        }

        var info = Enumerable.Range(0, count).Select(_ =>
        {
            var privateKey = new PrivateKey();
            return new
            {
                PrivateKey = PrivateKeyUtility.ToString(privateKey),
                privateKey.PublicKey,
                privateKey.Address,
            };
        }).ToArray();
        Out.WriteLineAsJson(info);
    }

    [CommandMethod]
    [CommandSummary("Displays the corresponding public key for the given private key")]
    public void Public(
        [CommandSummary("Speicifies the private key")]
        string privateKey)
    {
        var key = new PrivateKey(privateKey);
        var info = new
        {
            key.PublicKey,
        };
        Out.WriteLineAsJson(info);
    }

    [CommandMethod]
    [CommandSummary("Displays the address corresponding to the given private or public key")]
    public void Address(
        [CommandSummary("Speicifies the private key or public key")]
        string key)
    {
        var info = new
        {
            Address = GetAddress(key),
        };
        Out.WriteLineAsJson(info);
    }

    [CommandMethod]
    [CommandSummary("Derives a new address from the provided address and keyword")]
    public void Derive(
        [CommandSummary("Speicifies the address")]
        string address,
        [CommandSummary("Speicifies the keyword")]
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
        if (PrivateKeyUtility.TryParse(key, out var privateKey) == true)
        {
            return privateKey.Address;
        }

        if (PublicKey.FromHex(key) is { } publicKey)
        {
            return publicKey.Address;
        }

        throw new ArgumentException("Invalid key.", nameof(key));
    }
}

using Libplanet.Crypto;

namespace OnBoarding.ConsoleHost;

sealed class User
{
    private readonly PrivateKey _privateKey = new();

    public User(int index)
    {
        Address = _privateKey.ToAddress();
        Index = index;
    }

    public int Index { get; }

    public override string ToString() => $"{_privateKey.PublicKey}";

    public PrivateKey PrivateKey => _privateKey;

    public PublicKey PublicKey => _privateKey.PublicKey;

    public Address Address { get; }
}

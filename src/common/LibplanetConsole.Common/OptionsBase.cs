namespace LibplanetConsole.Common;

public abstract record class OptionsBase<T>
    where T : OptionsBase<T>
{
    public byte[] Signature { get; set; } = [];

    public T Sign(ISigner signer)
    {
        if (Signature.Length > 0)
        {
            throw new InvalidOperationException("Already signed.");
        }

        var signature = signer.Sign(this);
        return (T)this with { Signature = signature };
    }

    public bool TryVerify(IVerifier verifier)
    {
        if (Signature.Length == 0)
        {
            throw new InvalidOperationException("Not signed yet.");
        }

        var obj = this with { Signature = [] };
        var signature = Signature;
        return verifier.Verify(obj, signature);
    }

    public T Verify(IVerifier verifier)
    {
        if (Signature.Length == 0)
        {
            throw new InvalidOperationException("Not signed yet.");
        }

        var obj = this with { Signature = [] };
        var signature = Signature;
        if (verifier.Verify(obj, signature) != true)
        {
            throw new InvalidOperationException("Invalid signature.");
        }

        return (T)this;
    }
}

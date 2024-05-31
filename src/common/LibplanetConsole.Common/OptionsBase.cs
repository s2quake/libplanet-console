namespace LibplanetConsole.Common;

public abstract record class OptionsBase<T>
    where T : OptionsBase<T>
{
    public byte[] Signature { get; set; } = [];

    public T Sign(ISigner signer)
    {
        var signature = signer.Sign(this);
        return (T)this with { Signature = signature };
    }

    public bool Verify(IVerifier verifier)
    {
        var obj = this with { Signature = [] };
        var signature = Signature;
        return verifier.Verify(obj, signature);
    }
}

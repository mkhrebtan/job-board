namespace Domain.Abstraction;

public abstract record Id
{
    public Guid Value { get; private init; }

    protected Id()
    {
        Value = Guid.NewGuid();
    }
}

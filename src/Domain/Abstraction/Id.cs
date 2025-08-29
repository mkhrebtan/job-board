namespace Domain.Abstraction;

public abstract record Id
{
    public Guid Value { get; private init; }

    protected Id(Guid id)
    {
        Value = id;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}

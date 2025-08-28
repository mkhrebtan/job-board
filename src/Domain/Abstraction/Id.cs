namespace Domain.Abstraction;

public abstract record Id<T>
    where T : struct
{
    public T Value { get; private init; }

    protected Id(T id)
    {
        Value = id;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}

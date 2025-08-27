namespace Domain.Abstraction;

public abstract class AggregateRoot<T> : Entity<T>
    where T : Id
{
    protected AggregateRoot(T id)
        : base(id)
    {
    }
}

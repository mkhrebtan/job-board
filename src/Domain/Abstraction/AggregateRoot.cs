namespace Domain.Abstraction;

public abstract class AggregateRoot<T> : Entity<T>
    where T : Id<Guid>
{
    protected AggregateRoot(T id)
        : base(id)
    {
    }
}

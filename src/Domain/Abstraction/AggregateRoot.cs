namespace Domain.Abstraction;

public abstract class AggregateRoot<TId> : Entity<TId>
    where TId : Id
{
    protected AggregateRoot(TId id)
        : base(id)
    {
    }
}

namespace Domain.Abstraction;

public abstract class Entity<T> : IEquatable<Entity<T>>
    where T : Id<Guid>
{
    protected Entity(T id)
    {
        Id = id;
    }

    public T Id { get; protected set; }

    public override bool Equals(object? obj)
    {
        return obj is Entity<T> entity && Equals(entity);
    }

    public bool Equals(Entity<T>? other)
    {
        return other is not null && Id == other.Id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(GetType(), Id);
    }
}

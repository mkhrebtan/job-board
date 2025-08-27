using Domain.Abstraction;

namespace Domain.Repos;

public interface IRepository<TRoot>
    where TRoot : AggregateRoot
{
    Task<TRoot?> GetByIdAsync(Guid id, CancellationToken ct);

    void Add(TRoot root);

    void Update(TRoot root);

    void Remove(TRoot root);
}

using Domain.Abstraction;

namespace Domain.Repos;

public interface IRepository<TRoot, TId>
    where TRoot : AggregateRoot<TId>
    where TId : Id
{
    void Add(TRoot root);

    void Update(TRoot root);

    void Remove(TRoot root);
}
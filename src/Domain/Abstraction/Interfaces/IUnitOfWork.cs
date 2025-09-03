namespace Domain.Abstraction.Interfaces;

public interface IUnitOfWork
{
    Task CreateTransactionAsync();

    Task CommitTransactionAsync();

    Task RollbackTransactionAsync();

    Task SaveChangesAsync();
}

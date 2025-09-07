using Domain.Abstraction.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Persistence;

internal class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly ApplicationDbContext _context;

    private bool _disposed = false;

    private IDbContextTransaction? _contextTransaction;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task CreateTransactionAsync(CancellationToken cancellationToken = default)
    {
        _contextTransaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _contextTransaction!.CommitAsync();
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _contextTransaction!.RollbackAsync();
        await Task.Run(() =>
        {
            _contextTransaction.Dispose();
        });
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException(ex.Message, ex.InnerException);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _context.Dispose();
        }

        _disposed = true;
    }
}

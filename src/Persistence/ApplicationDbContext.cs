using Application.Abstraction.Events;
using Domain.Abstraction;
using Domain.Contexts.ApplicationContext.Aggregates;
using Domain.Contexts.IdentityContext.Aggregates;
using Domain.Contexts.JobPostingContext.Aggregates;
using Domain.Contexts.RecruitmentContext.Aggregates;
using Domain.Contexts.ResumePostingContext.Aggregates;
using Domain.Shared.Events;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class ApplicationDbContext : DbContext
{
    private readonly IDomainEventsDispatcher _domainEventsDispatcher;

    public ApplicationDbContext(IDomainEventsDispatcher domainEventsDispatcher)
    {
        _domainEventsDispatcher = domainEventsDispatcher;
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDomainEventsDispatcher domainEventsDispatcher)
        : base(options)
    {
        _domainEventsDispatcher = domainEventsDispatcher;
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<Company> Companies => Set<Company>();

    public DbSet<CompanyUser> CompanyUsers => Set<CompanyUser>();

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Vacancy> Vacancies => Set<Vacancy>();

    public DbSet<Resume> Resumes => Set<Resume>();

    public DbSet<VacancyApplication> VacancyApplications => Set<VacancyApplication>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await PublishDomainEventsAsync();

        int result = await base.SaveChangesAsync(cancellationToken);
        return result;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql();
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    private async Task PublishDomainEventsAsync()
    {
        var entries = ChangeTracker.Entries<IAggregateRoot>().ToList();

        var roots = entries
            .Select(entry => entry.Entity)
            .ToList();

        var domainEvents = roots
            .SelectMany(entity => entity.DomainEvents)
            .ToList();

        await _domainEventsDispatcher.DispatchAsync(domainEvents);
    }
}

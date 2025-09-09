using Application.Abstraction.Events;
using Domain.Abstraction;
using Domain.Contexts.ApplicationContext.Aggregates;
using Domain.Contexts.IdentityContext.Aggregates;
using Domain.Contexts.JobPostingContext.Aggregates;
using Domain.Contexts.RecruitmentContext.Aggregates;
using Domain.Contexts.ResumePostingContext.Aggregates;
using Domain.ReadModels.Resumes;
using Domain.ReadModels.Vacancies;
using Domain.Shared.Events;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class ApplicationDbContext : DbContext
{
    private readonly IDomainEventsDispatcher _domainEventsDispatcher;

    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDomainEventsDispatcher domainEventsDispatcher)
        : base(options)
    {
        _domainEventsDispatcher = domainEventsDispatcher;
    }

    #region Write

    public DbSet<User> Users => Set<User>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<Company> Companies => Set<Company>();

    public DbSet<CompanyUser> CompanyUsers => Set<CompanyUser>();

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Vacancy> Vacancies => Set<Vacancy>();

    public DbSet<Resume> Resumes => Set<Resume>();

    public DbSet<VacancyApplication> VacancyApplications => Set<VacancyApplication>();

    #endregion

    #region Read

    public DbSet<UserResumesReadModel> UserResumes => Set<UserResumesReadModel>();

    public DbSet<ResumeListingReadModel> ResumeListing => Set<ResumeListingReadModel>();

    public DbSet<VacancyListingReadModel> VacancyListing => Set<VacancyListingReadModel>();

    public DbSet<RegisteredVacanciesReadModel> RegisteredVacancies => Set<RegisteredVacanciesReadModel>();

    public DbSet<CompanyVacanciesReadModel> CompanyVacancies => Set<CompanyVacanciesReadModel>();

    public DbSet<VacancyApplicationsReadModel> VacancyApplicationsReadModels => Set<VacancyApplicationsReadModel>();

    #endregion

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        int result = await base.SaveChangesAsync(cancellationToken);

        await PublishDomainEventsAsync();

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
        var entries = ChangeTracker.Entries<IAggregateRoot>();

        var roots = entries
            .Select(entry => entry.Entity);

        var domainEvents = roots
            .SelectMany(entity =>
            {
                List<IDomainEvent> domainEvents = entity.DomainEvents.ToList();

                entity.ClearDomainEvents();

                return domainEvents;
            })
            .ToList();

        await _domainEventsDispatcher.DispatchAsync(domainEvents);
    }
}

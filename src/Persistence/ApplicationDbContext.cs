using Domain.Contexts.ApplicationContext.Aggregates;
using Domain.Contexts.IdentityContext.Aggregates;
using Domain.Contexts.JobPostingContext.Aggregates;
using Domain.Contexts.RecruitmentContext.Aggregates;
using Domain.Contexts.ResumePostingContext.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<Company> Companies => Set<Company>();

    public DbSet<CompanyUser> CompanyUsers => Set<CompanyUser>();

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Vacancy> Vacancies => Set<Vacancy>();

    public DbSet<Resume> Resumes => Set<Resume>();

    public DbSet<VacancyApplication> VacancyApplications => Set<VacancyApplication>();

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
}

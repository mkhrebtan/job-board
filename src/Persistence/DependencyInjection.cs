using Domain.Abstraction.Interfaces;
using Domain.Repos;
using Domain.Repos.Categories;
using Domain.Repos.Companies;
using Domain.Repos.CompanyUsers;
using Domain.Repos.RefreshTokens;
using Domain.Repos.Resumes;
using Domain.Repos.Users;
using Domain.Repos.Vacancies;
using Domain.Repos.VacancyApplications;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Repos;
using Persistence.Repos.Categories;
using Persistence.Repos.Companies;
using Persistence.Repos.CompanyUsers;
using Persistence.Repos.RefreshTokens;
using Persistence.Repos.Resumes;
using Persistence.Repos.Users;
using Persistence.Repos.Vacancies;
using Persistence.Repos.VacancyApplications;

namespace Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(config["CONNECTION_STRING"]));

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped(typeof(IRepository<,>), typeof(GenericRepository<,>));

        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<ICompanyUserRepository, CompanyUserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IResumeRepository, ResumeRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IVacancyRepository, VacancyRepository>();
        services.AddScoped<IVacancyApplicationRepository, VacancyApplicationRepository>();

        return services;
    }
}
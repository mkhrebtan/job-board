using Domain.Abstraction.Interfaces;
using Infrastructure.Parser;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IMarkdownParser, MarkdigParser>();
        return services;
    }
}

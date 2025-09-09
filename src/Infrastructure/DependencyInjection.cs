using Amazon.S3;
using API.Authentication;
using Application.Abstraction.Authentication;
using Application.Abstraction.Events;
using Application.Abstraction.Storage;
using Domain.Abstraction.Interfaces;
using Infrastructure.Authentication;
using Infrastructure.Events;
using Infrastructure.Parser;
using Infrastructure.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IMarkdownParser, MarkdigParser>();

        services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));

        services.AddAuthorization();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var jwtSettings = configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>();

                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings!.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(jwtSettings.SecretKey)),
                };
            });

        services.AddHttpContextAccessor();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<ITokenProvider, TokenProvider>();
        services.AddScoped<IUserContext, UserContext>();

        services.AddScoped<IDomainEventsDispatcher, DomainEventsDispatcher>();

        services.Configure<S3Settings>(options =>
        {
            options.BucketName = configuration.GetSection(S3Settings.SectionName)[nameof(S3Settings.BucketName)]!;
            options.Region = configuration.GetSection(S3Settings.SectionName)[nameof(S3Settings.Region)]!;
        });
        services.AddDefaultAWSOptions(configuration.GetAWSOptions());
        services.AddAWSService<IAmazonS3>();
        services.AddScoped<IStorage, S3Storage>();

        return services;
    }
}

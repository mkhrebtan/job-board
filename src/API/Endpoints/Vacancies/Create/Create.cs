using API.Authentication;
using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Vacancies.Create;
using Domain.Contexts.IdentityContext.Enums;

namespace API.Endpoints.Vacancies.Create;

internal sealed class Create : IEndpoint
{
    public record CreateVacancyRequest(
        string Title,
        string DescriptionMarkdown,
        decimal MinSalary,
        decimal? MaxSalary,
        string? SalaryCurrency,
        string Country,
        string City,
        string? Region,
        string? District,
        string? Address,
        decimal? Latitude,
        decimal? Longitude,
        string RecruiterFirstName,
        string RecruiterEmail,
        string RecruiterPhoneNumber,
        string RecruiterPhoneNumberRegionCode,
        bool IsDraft);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("vacancies/", async (
            CreateVacancyRequest request,
            ICommandHandler<CreateVacancyCommand, CreateVacancyCommandResponse> handler,
            IUserContext userContext,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateVacancyCommand(
                userContext.UserId,
                request.Title,
                request.DescriptionMarkdown,
                request.MinSalary,
                request.MaxSalary,
                request.SalaryCurrency,
                request.Country,
                request.City,
                request.Region,
                request.District,
                request.Address,
                request.Latitude,
                request.Longitude,
                request.RecruiterFirstName,
                request.RecruiterEmail,
                request.RecruiterPhoneNumber,
                request.RecruiterPhoneNumberRegionCode,
                request.IsDraft);
            var result = await handler.Handle(command, cancellationToken);
            return result.IsSuccess ? Results.Created($"vacancies/{result.Value.Id}", result.Value) : result.GetProblem();
        })
        .WithTags("Vacancies")
        .RequireAuthorization(policy => policy.RequireRole(
            UserRole.CompanyAdmin.Code,
            UserRole.CompanyEmployee.Code));
    }
}
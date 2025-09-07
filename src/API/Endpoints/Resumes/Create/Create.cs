using API.Authentication;
using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Commands.Resumes.Create;
using Application.Commands.Resumes.Dtos;
using Domain.Contexts.IdentityContext.Enums;

namespace API.Endpoints.Resumes.Create;

internal sealed class Create : IEndpoint
{
    internal sealed record CreateResumeRequest(
    string FirstName,
    string LastName,
    string Country,
    string City,
    string? Region,
    string? District,
    decimal? Latitude,
    decimal? Longitude,
    string Email,
    string PhoneNumber,
    string PhoneNumberRegionCode,
    string DesiredPositionTitle,
    decimal SalaryAmount,
    string SalaryCurrency,
    string SkillsDescripitonMarkdown,
    ICollection<string> EmploymentTypes,
    ICollection<string> WorkArrangements,
    ICollection<WorkExperienceDto> WorkExpetiences,
    ICollection<EducationDto> Educations,
    ICollection<LanguageSkillDto> Languages);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/resumes", async (
            CreateResumeRequest request,
            ICommandHandler<CreateResumeCommand, CreateResumeCommandResponse> handler,
            IUserContext userContext,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateResumeCommand(
                userContext.UserId,
                request.FirstName,
                request.LastName,
                request.Country,
                request.City,
                request.Region,
                request.District,
                request.Latitude,
                request.Longitude,
                request.Email,
                request.PhoneNumber,
                request.PhoneNumberRegionCode,
                request.DesiredPositionTitle,
                request.SalaryAmount,
                request.SalaryCurrency,
                request.SkillsDescripitonMarkdown,
                request.EmploymentTypes,
                request.WorkArrangements,
                request.WorkExpetiences,
                request.Educations,
                request.Languages);
            var result = await handler.Handle(command, cancellationToken);
            return result.IsSuccess ? Results.Ok(result.Value) : result.GetProblem();
        })
        .WithTags("Resumes")
        .RequireAuthorization(policy => policy.RequireRole(UserRole.JobSeeker.ToString()));
    }
}

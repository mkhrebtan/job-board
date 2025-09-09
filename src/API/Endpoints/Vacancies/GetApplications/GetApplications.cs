using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Queries.Vacancies.GetVacancyApplications;
using Domain.Contexts.IdentityContext.Enums;

namespace API.Endpoints.Vacancies.GetApplications;

internal sealed class GetApplications : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/vacancies/{vacancyId:guid}/applications", async (
            Guid vacancyId,
            int page,
            int pageSize,
            IQueryHandler<GetVacancyApplicationsQuery, GetVacancyApplicationsQueryResponse> queryHandler,
            CancellationToken cancellationToken) =>
        {
            var result = await queryHandler.Handle(new GetVacancyApplicationsQuery(vacancyId, page, pageSize), cancellationToken);
            return result.IsSuccess ? Results.Ok(result.Value) : result.GetProblem();
        })
        .WithTags("Vacancies")
        .RequireAuthorization(policy => policy.RequireRole(UserRole.CompanyAdmin.ToString(), UserRole.CompanyEmployee.ToString()));
    }
}

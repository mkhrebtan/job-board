using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Queries.Vacancies.GetAllCompanyVacancies;
using Domain.Contexts.IdentityContext.Enums;

namespace API.Endpoints.Vacancies.GetByCompany;

internal sealed class GetByCompany : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("vacancies/{companyId:guid}", async (
            IQueryHandler<GetAllCompanyVacanciesQuery, GetAllCompanyVacanciesQueryResponse> queryHandler,
            Guid companyId,
            CancellationToken cancellationToken) =>
        {
            var result = await queryHandler.Handle(new GetAllCompanyVacanciesQuery(companyId), cancellationToken);
            return result.IsSuccess ? Results.Ok(result.Value) : result.GetProblem();
        })
        .WithTags("Vacancies")
        .RequireAuthorization(policy => policy.RequireRole(UserRole.CompanyAdmin.ToString(), UserRole.CompanyEmployee.ToString()));
    }
}
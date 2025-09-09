using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Queries.Vacancies.GetPublishedCompanyVacancies;

namespace API.Endpoints.Vacancies.GetPublishedByCompany;

internal sealed class GetPublishedByCompany : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("vacancies/{companyId:guid}/published", async (
            IQueryHandler<GetPublishedCompanyVacanciesQuery, GetPublishedCompanyVacanciesQueryResponse> queryHandler,
            Guid companyId,
            CancellationToken cancellationToken) =>
        {
            var result = await queryHandler.Handle(new GetPublishedCompanyVacanciesQuery(companyId), cancellationToken);
            return result.IsSuccess ? Results.Ok(result.Value) : result.GetProblem();
        })
        .WithTags("Vacancies");
    }
}

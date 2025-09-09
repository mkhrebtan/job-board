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
            Guid? categoryId,
            bool? newFirst,
            int page,
            int pageSize,
            CancellationToken cancellationToken) =>
        {
            var result = await queryHandler.Handle(new GetPublishedCompanyVacanciesQuery(companyId, categoryId, newFirst, page, pageSize), cancellationToken);
            return result.IsSuccess ? Results.Ok(result.Value) : result.GetProblem();
        })
        .WithTags("Vacancies");
    }
}

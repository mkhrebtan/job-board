using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Queries.Vacancies.GetAllVacancies;

namespace API.Endpoints.Vacancies.GetAll;

internal sealed class GetAll : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("vacancies/", async (
            Guid[]? categoryIds,
            string? search,
            string? sortProperty,
            bool? sortDescending,
            decimal? minSalary,
            decimal? maxSalary,
            string? salaryCurrency,
            string? country,
            string? city,
            string? region,
            string? district,
            decimal? latitude,
            decimal? longitude,
            int page,
            int pageSize,
            IQueryHandler<GetAllVacanciesQuery, GetAllVacanciesQueryResponse> queryHandler,
            CancellationToken cancellationToken) =>
        {
            var result = await queryHandler.Handle(
                new GetAllVacanciesQuery(
                    categoryIds,
                    search,
                    sortProperty,
                    sortDescending,
                    minSalary,
                    maxSalary,
                    salaryCurrency,
                    country,
                    city,
                    region,
                    district,
                    latitude,
                    longitude,
                    page,
                    pageSize), cancellationToken);
            return result.IsSuccess ? Results.Ok(result.Value) : result.GetProblem();
        })
        .WithTags("Vacancies");
    }
}

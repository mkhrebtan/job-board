using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Queries.Resumes.GetAll;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints.Resumes.GetAll;

internal sealed class GetAll : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("resumes/", async (
            string? search,
            string? sortProperty,
            bool? sortDescending,
            bool? onlyWithSalary,
            bool? noExperience,
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
            [FromQuery] string[]? employmentTypes,
            [FromQuery] string[]? workArrangements,
            IQueryHandler<GetAllResumesQuery, GetAllResumesQueryResponse> queryHandler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllResumesQuery(
                search,
                sortProperty,
                sortDescending,
                onlyWithSalary,
                noExperience,
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
                pageSize,
                employmentTypes?.ToList(),
                workArrangements?.ToList());
            var result = await queryHandler.Handle(query, cancellationToken);
            return result.IsSuccess ? Results.Ok(result.Value) : result.GetProblem();
        })
        .WithTags("Resumes");
    }
}


using API.Extensions;
using Application.Abstractions.Messaging;
using Application.Queries.Categories;

namespace API.Endpoints.Categories.GetAll;

internal sealed class GetAll : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/categories", async (
            IQueryHandler<GetCategoriesQuery, GetCategoriesQueryResponse> queryHandler,
            CancellationToken cancellationToken) =>
        {
            var result = await queryHandler.Handle(new GetCategoriesQuery(), cancellationToken);
            return result.IsSuccess ? Results.Ok(result.Value) : result.GetProblem();
        })
        .WithTags("Categories");
    }
}

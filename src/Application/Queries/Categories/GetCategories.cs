using Application.Abstractions.Messaging;
using Domain.Repos.Categories;
using Domain.Shared.ErrorHandling;

namespace Application.Queries.Categories;

public record GetCategoriesQuery() : IQuery<GetCategoriesQueryResponse>;

public record GetCategoriesQueryResponse(IEnumerable<CategoryDto> Categories);

public record CategoryDto(Guid Id, string Name);

internal sealed class GetCategoriesQueryHandler : IQueryHandler<GetCategoriesQuery, GetCategoriesQueryResponse>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoriesQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<GetCategoriesQueryResponse>> Handle(GetCategoriesQuery query, CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);
        return Result<GetCategoriesQueryResponse>.Success(new GetCategoriesQueryResponse(categories.Select(c => new CategoryDto(c.Id.Value, c.Name))));
    }
}

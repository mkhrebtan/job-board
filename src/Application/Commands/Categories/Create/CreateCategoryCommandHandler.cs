using Application.Abstractions.Messaging;
using Domain.Abstraction.Interfaces;
using Domain.Repos.Categories;
using Domain.Services;
using Domain.Shared.ErrorHandling;

namespace Application.Commands.Categories.Create;

internal class CreateCategoryCommandHandler : ICommandHandler<CreateCategoryCommand, CreateCategoryRepsonse>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly CategoryService _categoryService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _categoryService = new CategoryService(_categoryRepository);
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreateCategoryRepsonse>> Handle(CreateCategoryCommand command, CancellationToken cancellationToken = default)
    {
        var creationResult = await _categoryService.CreateCategoryAsync(command.Name, cancellationToken);
        if (creationResult.IsFailure)
        {
            return Result<CreateCategoryRepsonse>.Failure(creationResult.Error);
        }

        var category = creationResult.Value;
        _categoryRepository.Add(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<CreateCategoryRepsonse>.Success(new CreateCategoryRepsonse(category.Id.Value, category.Name));
    }
}

using Application.Abstractions.Messaging;
using Application.Commands.Categories.Create;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.JobPostingContext.IDs;
using Domain.Repos.Categories;
using Domain.Services;
using Domain.Shared.ErrorHandling;

namespace Application.Commands.Categories.UpdateName;

internal sealed class UpdateCategoryNameCommandHandler : ICommandHandler<UpdateCategoryNameCommand>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly CategoryService _categoryService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCategoryNameCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _categoryService = new CategoryService(_categoryRepository);
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateCategoryNameCommand command, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(new CategoryId(command.Id), cancellationToken);
        if (category is null)
        {
            return Result.Failure(Error.NotFound("Category.NotFound", $"Category with ID '{command.Id}' not found."));
        }

        var updateResult = await _categoryService.UpdateCategoryNameAsync(category, command.Name, cancellationToken);
        if (updateResult.IsFailure)
        {
            return Result.Failure(updateResult.Error);
        }

        _categoryRepository.Update(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

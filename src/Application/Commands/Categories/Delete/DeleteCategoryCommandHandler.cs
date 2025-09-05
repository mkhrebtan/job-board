using Application.Abstractions.Messaging;
using Domain.Abstraction.Interfaces;
using Domain.Repos.Categories;
using Domain.Shared.ErrorHandling;

namespace Application.Commands.Categories.Delete;

internal class DeleteCategoryCommandHandler : ICommandHandler<DeleteCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteCategoryCommand command, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(command.Id, cancellationToken);
        if (category is null)
        {
            return Result.Failure(Error.NotFound("Category.NotFound", "The category was not found."));
        }

        if (await _categoryRepository.HasAssignedVacancies(category, cancellationToken))
        {
            return Result.Failure(Error.Validation("Category.HasAssignedVacancies", "The category has assigned vacancies and cannot be deleted."));
        }

        _categoryRepository.Remove(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
